using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Tingle.Extensions.MongoDB;

/// <summary>
/// Helper for performing creation.
/// </summary>
/// <typeparam name="TContext">The type of context to be used.</typeparam>
internal class DatabaseSetup<TContext>(IServiceScopeFactory scopeFactory, ILogger<DatabaseSetup<TContext>> logger) : IHostedService where TContext : MongoDbContext
{
    private readonly IServiceScopeFactory scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var provider = scope.ServiceProvider;

        // Check if explicitly told to do creation
        var environment = provider.GetRequiredService<IHostEnvironment>();
        var configuration = provider.GetRequiredService<IConfiguration>();
        if (bool.TryParse(configuration["MONGO_CREATE_DATABASE"], out var b) && b)
        {
            // Create database
            logger.LogInformation("Creating MongoDB database ...");
            var context = provider.GetRequiredService<TContext>();
            await context.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Completed MongoDB database creation.");
        }
        else
        {
            logger.LogDebug("Database creation skipped.");
            return;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
