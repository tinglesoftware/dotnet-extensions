using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Options for configuring <see cref="MongoDbContext"/> instances.
/// 
/// The options to be used by a <see cref="MongoDbContext"/>. You normally
/// override <see cref="MongoDbContext.OnConfiguring(MongoDbContextOptionsBuilder)"/>
/// or use a <see cref="MongoDbContextOptionsBuilder"/> to create instances
/// of this class and it is not designed to be directly constructed in your application
/// code.
/// </summary>
public abstract class MongoDbContextOptions
{
    private readonly Dictionary<Type, object> metadata = [];

    /// <summary>
    /// The type of context that these options are for. Will return <see cref="MongoDbContext"/>
    /// if the options are not built for a specific derived context.
    /// </summary>
    public abstract Type ContextType { get; }

    /// <summary>Add metadata to <see cref="MongoDbContextOptions"/>.</summary>
    /// <typeparam name="T">The type of metadata.</typeparam>
    /// <param name="value">The value to be added.</param>
    public void AddMetadata<T>(T value) where T : class => metadata.Add(typeof(T), value);

    /// <summary>Gets the metadata from <see cref="MongoDbContextOptions"/>.</summary>
    /// <typeparam name="T">The type of metadata.</typeparam>
    /// <returns></returns>
    public T GetMetadata<T>() where T : class => (T)metadata[typeof(T)];

    /// <summary>Gets the metadata from <see cref="MongoDbContextOptions"/>.</summary>
    /// <typeparam name="T">The type of metadata.</typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetMetadata<T>([NotNullWhen(true)] out T? value) where T : class
    {
        if (metadata.TryGetValue(typeof(T), out var raw))
        {
            if (raw is T t)
            {
                value = t;
                return true;
            }
        }

        value = null;
        return false;
    }

    internal IServiceProvider GetServiceProvider() => GetMetadata<IServiceProvider>();

    internal MongoUrl GetMongoUrl() => GetMetadata<MongoUrl>();
    internal bool TryGetMongoClientSettings([NotNullWhen(true)] out MongoClientSettings? settings) => TryGetMetadata(out settings);
    internal bool TryGetMongoUrl([NotNullWhen(true)] out MongoUrl? url) => TryGetMetadata(out url);
}

/// <summary>
/// The options to be used by a <see cref="MongoDbContext"/>. You normally
/// override <see cref="MongoDbContext.OnConfiguring(MongoDbContextOptionsBuilder)"/>
/// or use a <see cref="MongoDbContextOptionsBuilder{TContext}"/> to create instances
/// of this class and it is not designed to be directly constructed in your application
/// code.
/// </summary>
/// <typeparam name="TContext">The type of the context these options apply to.</typeparam>
public class MongoDbContextOptions<TContext> : MongoDbContextOptions where TContext : MongoDbContext
{
    /// <summary>
    /// The type of context that these options are for (TContext).
    /// </summary>
    public override Type ContextType => typeof(TContext);
}

/// <summary>
/// Provides a simple API surface for configuring <see cref="MongoDbContextOptions"/>.
/// You can use <see cref="MongoDbContextOptionsBuilder"/> to configure
/// a context by overriding <see cref="MongoDbContext.OnConfiguring(MongoDbContextOptionsBuilder)"/>
/// or creating a <see cref="MongoDbContextOptions"/> externally and passing
/// it to the context constructor.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MongoDbContextOptionsBuilder" /> class to further configure
/// a given <see cref="MongoDbContextOptions" />.
/// </remarks>
/// <param name="options">The options to be configured.</param>
public class MongoDbContextOptionsBuilder(MongoDbContextOptions options)
{
    private readonly MongoDbContextOptions options = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbContextOptionsBuilder" /> class with no options set.
    /// </summary>
    public MongoDbContextOptionsBuilder()
        : this(new MongoDbContextOptions<MongoDbContext>())
    {
    }

    /// <summary>
    /// Gets the options being configured.
    /// </summary>
    public virtual MongoDbContextOptions Options => options;

    /// <summary>
    /// Sets the <see cref="IServiceProvider"/> from which application services will be obtained.
    /// This is done automatically when using 'AddDbContext' , so
    /// it is rare that this method needs to be called.
    /// </summary>
    /// <param name="serviceProvider">The service provider to be used.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public virtual MongoDbContextOptionsBuilder UseApplicationServiceProvider(IServiceProvider serviceProvider)
    {
        options.AddMetadata(serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)));
        return this;
    }

    /// <summary>
    /// Sets the <see cref="MongoUrl"/> to use when configuring the context.
    /// </summary>
    /// <param name="url">The <see cref="MongoUrl"/> to be used</param>
    /// <param name="instrumentationOptions">The options to use for instrumentation.</param>
    /// <returns></returns>
    public virtual MongoDbContextOptionsBuilder UseMongoUrl(MongoUrl url, InstrumentationOptions? instrumentationOptions = null)
    {
        ArgumentNullException.ThrowIfNull(url);
        if (string.IsNullOrWhiteSpace(url.DatabaseName))
        {
            throw new ArgumentException("The database must be specified", nameof(url));
        }

        options.AddMetadata(url);

        // add the default event subscriber
        ConfigureMongoClientSettings(settings =>
        {
            settings.ClusterConfigurator = builder =>
            {
                builder.Subscribe(new DiagnosticsActivityEventSubscriber(instrumentationOptions ?? new() { CaptureCommandText = true }));
            };
        });

        return this;
    }

    /// <summary>
    /// Sets the <see cref="MongoUrl"/> to use when configuring the context by
    /// parsing the <paramref name="connectionString"/>.
    /// </summary>
    /// <param name="connectionString">
    /// A valid Mongo db collection in the format
    /// <c>mongodb://[username:password@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[database][?options]]</c>
    /// e.g. <c>mongodb://localhost:27017/myDatabase</c>
    /// </param>
    /// <param name="instrumentationOptions">The options to use for instrumentation.</param>
    /// <returns></returns>
    public virtual MongoDbContextOptionsBuilder UseMongoConnectionString(string connectionString, InstrumentationOptions? instrumentationOptions = null)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        return UseMongoUrl(new MongoUrl(connectionString), instrumentationOptions);
    }

    /// <summary>
    /// Further configure the existing instance of <see cref="MongoClientSettings"/>.
    /// </summary>
    /// <param name="configure">An <see cref="Action{T}"/> to further modify the <see cref="MongoClientSettings"/> instance.</param>
    /// <returns></returns>
    public virtual MongoDbContextOptionsBuilder ConfigureMongoClientSettings(Action<MongoClientSettings> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        // Ensure we have settings
        if (!options.TryGetMongoClientSettings(out var settings))
        {
            if (!options.TryGetMongoUrl(out var url))
            {
                throw new InvalidOperationException("The connection string or URL must be configured before the settings are added");
            }

            settings = MongoClientSettings.FromUrl(url);
            options.AddMetadata(settings);
        }

        // Invoke the action
        configure(settings);
        return this;
    }
}

/// <summary>
/// Provides a simple API surface for configuring <see cref="MongoDbContextOptions{TContext}"/>.
/// You can use <see cref="MongoDbContextOptionsBuilder"/> to configure
/// a context by overriding <see cref="MongoDbContext.OnConfiguring(MongoDbContextOptionsBuilder)"/>
/// or creating a <see cref="MongoDbContextOptions"/> externally and passing
/// it to the context constructor.
/// </summary>
/// <typeparam name="TContext">The type of context to be configured.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="MongoDbContextOptionsBuilder{TContext}" /> class to further configure
/// a given <see cref="MongoDbContextOptions" />.
/// </remarks>
/// <param name="options">The options to be configured.</param>
public class MongoDbContextOptionsBuilder<TContext>(MongoDbContextOptions<TContext> options) : MongoDbContextOptionsBuilder(options) where TContext : MongoDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbContextOptionsBuilder{TContext}" /> class with no options set.
    /// </summary>
    public MongoDbContextOptionsBuilder() : this(new MongoDbContextOptions<TContext>()) { }

    /// <summary>
    /// Gets the options being configured.
    /// </summary>
    public new virtual MongoDbContextOptions<TContext> Options
        => (MongoDbContextOptions<TContext>)base.Options;

    /// <summary>
    /// Sets the <see cref="IServiceProvider" /> from which application services will be obtained. This
    /// is done automatically when using 'AddDbContext', so it is rare that this method needs to be called.
    /// </summary>
    /// <param name="serviceProvider">The service provider to be used.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public new virtual MongoDbContextOptionsBuilder<TContext> UseApplicationServiceProvider(IServiceProvider serviceProvider)
        => (MongoDbContextOptionsBuilder<TContext>)base.UseApplicationServiceProvider(serviceProvider);

    /// <summary>
    /// Sets the <see cref="MongoUrl"/> to use when configuring the context.
    /// </summary>
    /// <param name="url">The <see cref="MongoUrl"/> to be used</param>
    /// <param name="instrumentationOptions">The options to use for instrumentation.</param>
    /// <returns></returns>
    public new virtual MongoDbContextOptionsBuilder<TContext> UseMongoUrl(MongoUrl url, InstrumentationOptions? instrumentationOptions = null)
        => (MongoDbContextOptionsBuilder<TContext>)base.UseMongoUrl(url, instrumentationOptions);

    /// <summary>
    /// Sets the <see cref="MongoUrl"/> to use when configuring the context by
    /// parsing the <paramref name="connectionString"/>.
    /// </summary>
    /// <param name="connectionString">
    /// A valid Mongo db collection in the format
    /// <c>mongodb://[username:password@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[database][?options]]</c>
    /// e.g. <c>mongodb://localhost:27017/myDatabase</c>
    /// </param>
    /// <param name="instrumentationOptions">The options to use for instrumentation.</param>
    /// <returns></returns>
    public new virtual MongoDbContextOptionsBuilder<TContext> UseMongoConnectionString(string connectionString, InstrumentationOptions? instrumentationOptions=null)
        => (MongoDbContextOptionsBuilder<TContext>)base.UseMongoConnectionString(connectionString, instrumentationOptions);

    /// <summary>
    /// Further configure the existing instance of <see cref="MongoClientSettings"/>.
    /// </summary>
    /// <param name="configure">An <see cref="Action{T}"/> to further modify the <see cref="MongoClientSettings"/> instance.</param>
    /// <returns></returns>
    public new virtual MongoDbContextOptionsBuilder<TContext> ConfigureMongoClientSettings(Action<MongoClientSettings> configure)
        => (MongoDbContextOptionsBuilder<TContext>)base.ConfigureMongoClientSettings(configure);
}
