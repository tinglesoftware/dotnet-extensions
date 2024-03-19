using Microsoft.ApplicationInsights.Extensibility;
using System.Diagnostics;
using Tingle.AspNetCore.ApplicationInsights;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for application insights
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="ITelemetryInitializer"/> that collects details about the request source
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationInsightsTelemetryExtras(this IServiceCollection services)
    {
        // Required to resolve the request from the HttpContext
        services.AddHttpContextAccessor();

        // according to docs link below, this registration should be singleton
        // https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core#adding-telemetryinitializers
        return services.AddSingleton<ITelemetryInitializer, ExtrasTelemetryInitializer>();
    }

    /// <summary>
    /// Adds <see cref="ITelemetryInitializer"/> that collects all request headers
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationInsightsTelemetryHeaders(this IServiceCollection services)
    {
        // Required to resolve the request from the HttpContext
        services.AddHttpContextAccessor();

        // according to docs link below, this registration should be singleton
        // https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core#adding-telemetryinitializers
        return services.AddSingleton<ITelemetryInitializer, HeadersTelemetryInitializer>();
    }

    /// <summary>
    /// Adds dependency collector for the new <see cref="ActivitySource"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="activities"></param>
    /// <param name="samplingResult"></param>
    /// <returns></returns>
    public static IServiceCollection AddActivitySourceDependencyCollector(this IServiceCollection services,
                                                                          IEnumerable<string> activities,
                                                                          ActivitySamplingResult samplingResult = ActivitySamplingResult.PropagationData)
    {
        return services.AddActivitySourceDependencyCollector(activities.ToDictionary(a => a, _ => samplingResult));
    }

    /// <summary>
    /// Adds dependency collector for the new <see cref="ActivitySource"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="activities"></param>
    /// <returns></returns>
    public static IServiceCollection AddActivitySourceDependencyCollector(this IServiceCollection services,
                                                                          IDictionary<string, ActivitySamplingResult> activities)
    {
        return services.AddHostedService(p => ActivatorUtilities.CreateInstance<ActivitySourceDependencyCollector>(p, [activities]));
    }
}
