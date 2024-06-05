using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Tingle.Extensions.EntityFrameworkCore;

/// <summary>
/// Helper for performing migrations or creation.
/// </summary>
/// <typeparam name="TContext">The type of context to be used.</typeparam>
[RequiresDynamicCode(MessageStrings.MigrationsRequiresDynamicCodeMessage)]
public class DatabaseSetup<TContext> : IHostedService where TContext : DbContext
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger logger;

    /// <summary>
    /// Creates an instance of <see cref="DatabaseSetup{TContext}"/>.
    /// </summary>
    /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> with which to create <see cref="IServiceScope"/> instances.</param>
    /// <param name="logger">The <see cref="ILogger"/> instance to use.</param>
    public DatabaseSetup(IServiceScopeFactory scopeFactory, ILogger<DatabaseSetup<TContext>> logger) : this(scopeFactory, (ILogger)logger) { }

    /// <summary>
    /// Creates an instance of <see cref="DatabaseSetup{TContext}"/>.
    /// </summary>
    /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> with which to create <see cref="IServiceScope"/> instances.</param>
    /// <param name="logger">The <see cref="ILogger"/> instance to use.</param>
    protected DatabaseSetup(IServiceScopeFactory scopeFactory, ILogger logger)
    {
        this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var provider = scope.ServiceProvider;

        // Check if explicitly told to do migrations or creation
        var environment = provider.GetRequiredService<IHostEnvironment>();
        var configuration = provider.GetRequiredService<IConfiguration>();
        if (bool.TryParse(configuration["EFCORE_PERFORM_MIGRATIONS"], out var b) && b)
        {
            // Perform migrations
            logger.LogInformation("Performing EfCore migrations ...");
            var context = provider.GetRequiredService<TContext>();
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Completed EfCore migrations.");
        }
        else if (bool.TryParse(configuration["EFCORE_CREATE_DATABASE"], out b) && b)
        {
            // Create database
            logger.LogInformation("Creating EfCore database ...");
            var context = provider.GetRequiredService<TContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Completed EfCore database creation.");
        }
        else
        {
            logger.LogDebug("Database migrations/creation skipped.");
            return;
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
