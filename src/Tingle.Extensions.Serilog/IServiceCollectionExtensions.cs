using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Extension methods for <see cref="IServiceCollection"/>.</summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Add Serilog services via <see cref="SerilogBuilder"/>.
    /// This replaces the default <see cref="ILoggerProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to add services to.</param>
    /// <returns>An <see cref="SerilogBuilder"/> to continue setting up Serilog.</returns>
    /// <remarks>
    /// By default, this sets up destructuring of simple exceptions, sensitive data masking, reading of log levels
    /// from the <c>Logging:</c> configuration section, writing to console, debug and SEQ.
    /// For more specifics you can inspect <see cref="SerilogBuilder"/>.
    /// The logger will be shut down when application services are disposed.
    /// </remarks>
    public static SerilogBuilder AddSerilog(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return new SerilogBuilder(services);
    }

    /// <summary>
    /// Add Serilog services via <see cref="SerilogBuilder"/>.
    /// This replaces the default <see cref="ILoggerProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to add services to.</param>
    /// <param name="setupAction">An optional action for setting up Serilog.</param>
    /// <returns></returns>
    /// <remarks>
    /// By default, this sets up destructuring of simple exceptions, sensitive data masking, reading of log levels
    /// from the <c>Logging:</c> configuration section, writing to console, debug and SEQ.
    /// For more specifics you can inspect <see cref="SerilogBuilder"/>.
    /// The logger will be shut down when application services are disposed.
    /// </remarks>
    public static IServiceCollection AddSerilog(this IServiceCollection services, Action<SerilogBuilder>? setupAction = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = services.AddSerilog();

        setupAction?.Invoke(builder);

        return services;
    }
}
