using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Tingle.Extensions.MongoDB.Diagnostics;

internal class MongoDbContextHealthCheck<TContext> : IHealthCheck where TContext : MongoDbContext
{
    private readonly IMongoDatabase database;
    public MongoDbContextHealthCheck(TContext context)
    {
        database = context.Database ?? throw new ArgumentNullException(nameof(database));
    }

    /// <inheritdoc/>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using (await database.ListCollectionsAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
            {

            }

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
