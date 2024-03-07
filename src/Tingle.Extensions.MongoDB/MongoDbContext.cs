using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace MongoDB.Driver;

/// <summary>
/// An abstraction for a database context backed by MongoDB
/// </summary>
public abstract class MongoDbContext
{
    internal const System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes DynamicallyAccessedMemberTypes
        = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors
        | System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.NonPublicConstructors
        | System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties;

    private readonly ConcurrentDictionary<CollectionKeyEntry, object> collections = new();
    private readonly MongoDbContextOptions options;
    private readonly object _lockObjectClient = new();
    private readonly object _lockObjectDatabase = new();

    private IMongoClient? client;
    private IMongoDatabase? database;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbContext"/> class.
    /// The <see cref="OnConfiguring(MongoDbContextOptionsBuilder)"/>
    /// method will be called to configure the database (and other options) to be used
    /// for this context.
    /// </summary>
    protected MongoDbContext() : this(new MongoDbContextOptions<MongoDbContext>()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbContext"/> class
    /// using the specified options. he <see cref="OnConfiguring(MongoDbContextOptionsBuilder)"/>
    /// method will still be called to allow further configuration of the options.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public MongoDbContext(MongoDbContextOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        if (!options.ContextType.IsAssignableFrom(GetType()))
        {
            throw new InvalidOperationException($"Non generic options used with '{GetType().Name}' are not supported");
        }

        var provider = options.GetServiceProvider();
        Logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());

        var optionsBuilder = new MongoDbContextOptionsBuilder(options);
        OnConfiguring(optionsBuilder);
    }

    ///
    public string? DatabaseName { get; protected set; }

    ///
    public IMongoClient Client
    {
        get
        {
            if (client is not null) return client;

            lock (_lockObjectClient)
            {
                if (client is null)
                {
                    // ensure we have a MongoUrl
                    if (!options.TryGetMongoUrl(out var url))
                    {
                        throw new InvalidOperationException("The connection string or URL must be configured.");
                    }

                    // get the MongoClientSettings or create from the MongoUrl
                    if (!options.TryGetMongoClientSettings(out var settings))
                    {
                        settings = MongoClientSettings.FromUrl(url);
                    }

                    client = new MongoClient(settings);
                    DatabaseName = url.DatabaseName;
                }
            }

            return client;
        }
    }

    ///
    public IMongoDatabase Database
    {
        get
        {
            if (database is not null) return database;

            lock (_lockObjectDatabase)
            {
                database ??= Client.GetDatabase(DatabaseName);
            }

            return database;
        }
    }

    ///
    protected ILogger Logger { get; }

    /// <summary>Gets a collection.</summary>
    /// <typeparam name="T">The document type.</typeparam>
    /// <param name="name">The name of the collection.</param>
    /// <param name="settings">The settings for the collection.</param>
    /// <returns>An implementation of a collection.</returns>
    /// <exception cref="OverflowException">
    /// The dictionary already contains the maximum number of elements (<see cref="int.MaxValue"/>).
    /// </exception>
    protected IMongoCollection<T> Collection<T>(string name, MongoCollectionSettings? settings = null)
        => Collection<T>(name: name, databaseName: null, settings: settings);

    /// <summary>Gets a collection.</summary>
    /// <typeparam name="T">The document type.</typeparam>
    /// <param name="name">The name of the collection.</param>
    /// <param name="databaseName">The name of the database, if none is provided, the one in the connection string or <see cref="MongoUrl"/> is used.</param>
    /// <param name="settings">The settings for the collection.</param>
    /// <returns>An implementation of a collection.</returns>
    /// <exception cref="OverflowException">
    /// The dictionary already contains the maximum number of elements (<see cref="int.MaxValue"/>).
    /// </exception>
    protected IMongoCollection<T> Collection<T>(string name, string? databaseName, MongoCollectionSettings? settings = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        var db = string.IsNullOrWhiteSpace(databaseName) ? Database : Client.GetDatabase(databaseName);
        var key = new CollectionKeyEntry(db.DatabaseNamespace.DatabaseName, name, typeof(T).FullName);
        return (IMongoCollection<T>)collections.GetOrAdd(key, k => db.GetCollection<T>(name, settings));
    }

    #region Creation

    /// <summary>Ensure the collections, indexes and schema validations are created.</summary>
    public void EnsureCreated() => EnsureCreated(Database);

    /// <summary>Ensure the collections, indexes and schema validations are created.</summary>
    /// <param name="cancellationToken"></param>
    public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        => await EnsureCreatedAsync(Database, cancellationToken).ConfigureAwait(false);

    /// <summary>Ensure the collections, indexes and schema validations are created.</summary>
    /// <param name="database">The database to use.</param>
    protected void EnsureCreated(IMongoDatabase database) => EnsureCreatedAsync(database).GetAwaiter().GetResult();

    /// <summary>Ensure the collections, indexes and schema validations are created.</summary>
    /// <param name="database">The <see cref="IMongoDatabase"/> to use.</param>
    /// <param name="cancellationToken"></param>
    protected virtual Task EnsureCreatedAsync(IMongoDatabase database, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    #endregion

    /// <summary>
    /// Override this method to configure the database (and other options) to be used
    /// for this context. This method is called for each instance of the context that
    /// is created. The base implementation does nothing.
    /// </summary>
    /// <param name="optionsBuilder">
    /// A builder used to create or modify options for this context.
    /// Extensions typically define extension methods on this object that allow you to configure the context.
    /// </param>
    protected internal virtual void OnConfiguring(MongoDbContextOptionsBuilder optionsBuilder) { }

    #region Transactions

    /// <summary>Execute a given set of operations in a session with the transaction model.</summary>
    /// <param name="callbackAsync">The func to be executed within in the transaction.</param>
    /// <param name="options">The <see cref="ClientSessionOptions"/> for creating the session.</param>
    /// <param name="transactionOptions">The <see cref="TransactionOptions"/> for the transaction.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResult> WithTransactionAsync<TResult>(Func<IClientSessionHandle, CancellationToken, Task<TResult>> callbackAsync,
                                                             ClientSessionOptions? options = null,
                                                             TransactionOptions? transactionOptions = null,
                                                             CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callbackAsync);

        var client = Client;

        // Create a session object that is used when leveraging transactions
        using var session = await client.StartSessionAsync(options, cancellationToken).ConfigureAwait(false);

        // For local or standalone servers, transactions are not supported
        var isLocal = client.Settings.Servers.Count() == 1 && client.Settings.Server.ToString().Contains("localhost");
        if (isLocal)
        {
            Logger.StandaloneServerNotSupported();
            return await callbackAsync(session, cancellationToken).ConfigureAwait(false);
        }

        return await session.WithTransactionAsync(callbackAsync: callbackAsync,
                                                  transactionOptions: transactionOptions,
                                                  cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Execute a given set of operations in a session with the transaction model.</summary>
    /// <param name="func">The func to be executed within in the transaction.</param>
    /// <param name="options">The <see cref="ClientSessionOptions"/> for creating the session.</param>
    /// <param name="transactionOptions">The <see cref="TransactionOptions"/> for the transaction.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task WithTransactionAsync(Func<IClientSessionHandle, CancellationToken, Task> func,
                                           ClientSessionOptions? options = null,
                                           TransactionOptions? transactionOptions = null,
                                           CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(func);

        async Task<string> callbackAsync(IClientSessionHandle session, CancellationToken cancellationToken)
        {
            await func(session, cancellationToken).ConfigureAwait(false);
            return "Done";
        }

        await WithTransactionAsync(callbackAsync: callbackAsync,
                                   options: options,
                                   transactionOptions: transactionOptions,
                                   cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #endregion

    private sealed record CollectionKeyEntry
    {
        public CollectionKeyEntry(string databaseName, string collectionName, string? type)
        {
            DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            CollectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
            Type = type;
        }

        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string? Type { get; }
    }
}
