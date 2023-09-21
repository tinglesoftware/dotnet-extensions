using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting;

/// <summary>Extension methods for <see cref="IHostBuilder"/>.</summary>
public static class IHostBuilderExtensions
{
    /// <summary>
    /// Add Serilog services via <see cref="SerilogBuilder"/>.
    /// This replaces the default <see cref="ILoggerProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> builder to configure.</param>
    /// <param name="setupAction">An optional action for setting up Serilog.</param>
    /// <remarks>
    /// By default, this sets up destructuring of simple exceptions, sensitive data masking, reading of log levels
    /// from the <c>Logging:</c> configuration section, writing to console, debug and SEQ.
    /// For more specifics you can inspect <see cref="SerilogBuilder"/>.
    /// The logger will be shut down when application services are disposed.
    /// <br/>
    /// <br/>
    /// A <see cref="HostBuilderContext"/> is supplied so that configuration and hosting information can be used.
    /// </remarks>
    public static IHostBuilder UseSerilog(this IHostBuilder builder, Action<SerilogBuilder> setupAction)
        => builder.UseSerilog((_, builder) => setupAction?.Invoke(builder));

    /// <summary>
    /// Add Serilog services via <see cref="SerilogBuilder"/>.
    /// This replaces the default <see cref="ILoggerProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> builder to configure.</param>
    /// <param name="setupAction">An optional action for setting up Serilog.</param>
    /// <remarks>
    /// By default, this sets up destructuring of simple exceptions, sensitive data masking, reading of log levels
    /// from the <c>Logging:</c> configuration section, writing to console, debug and SEQ.
    /// For more specifics you can inspect <see cref="SerilogBuilder"/>.
    /// The logger will be shut down when application services are disposed.
    /// <br/>
    /// <br/>
    /// A <see cref="HostBuilderContext"/> is supplied so that configuration and hosting information can be used.
    /// </remarks>
    public static IHostBuilder UseSerilog(this IHostBuilder builder, Action<HostBuilderContext, SerilogBuilder>? setupAction = null)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        return builder.ConfigureServices((context, services) => services.AddSerilog(builder => setupAction?.Invoke(context, builder)));
    }
}
