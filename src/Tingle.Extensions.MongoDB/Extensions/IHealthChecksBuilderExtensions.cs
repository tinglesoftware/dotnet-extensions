using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Tingle.Extensions.MongoDB.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Extension methods for <see cref="IHealthChecksBuilder"/>.</summary>
public static class IHealthChecksBuilderExtensions
{
    /// <summary>Add a health check for an instance of <see cref="MongoDbContext"/>.</summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/> to add to.</param>
    /// <param name="name">The health check name.</param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check reports a failure.
    /// If the provided value is <see langword="null"/>, then <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used to filter health checks.</param>
    /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    public static IHealthChecksBuilder AddMongoDbContextCheck<TContext>(this IHealthChecksBuilder builder,
                                                                        string? name = null,
                                                                        HealthStatus? failureStatus = null,
                                                                        IEnumerable<string>? tags = null,
                                                                        TimeSpan? timeout = null)
        where TContext : MongoDbContext
    {
        name ??= typeof(TContext).Name;
        return builder.AddCheck<MongoDbContextHealthCheck<TContext>>(name, failureStatus, tags, timeout);
    }
}
