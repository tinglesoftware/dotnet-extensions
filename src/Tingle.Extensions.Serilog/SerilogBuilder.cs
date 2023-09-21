using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Formatting.Compact;
using Tingle.Extensions.Serilog;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A builder class for adding and configuring Serilog for Tingle
/// </summary>
public class SerilogBuilder
{
    /// <summary>
    /// Creates an instance of <see cref="SerilogBuilder"/>.
    /// </summary>
    /// <param name="services"></param>
    public SerilogBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));

        services.AddSerilog(ConfigureSerilog, preserveStaticLogger: false, writeToProviders: false);
    }

    /// <summary>
    /// The instance of <see cref="IServiceCollection"/> that this builder instance adds to.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>Configure destructuring with more than the defaults.</summary>
    /// <param name="configure"></param>
    public SerilogBuilder ConfigureDestructuring(Action<DestructuringOptionsBuilder> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        return Configure(options => options.ConfigureDestructuringOptions.Add(configure));
    }

    /// <summary>Configure sensitive data masking with more than the defaults.</summary>
    /// <param name="configure"></param>
    public SerilogBuilder ConfigureSensitiveDataMasking(Action<SensitiveDataEnricherOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        return Configure(options => options.ConfigureSensitiveDataEnricherOptions.Add(configure));
    }

    /// <summary>Configure the <see cref="LoggerConfiguration"/> on items not provided in the builder such as adding new providers.</summary>
    /// <param name="configure"></param>
    public SerilogBuilder ConfigureLoggerConfiguration(Action<LoggerConfiguration> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        return Configure(options => options.ConfigureLoggerConfiguration.Add(configure));
    }

    private SerilogBuilder Configure(Action<SerilogBuilderOptions> configure)
    {
        Services.Configure(configure);
        return this;
    }

    private void ConfigureSerilog(IServiceProvider serviceProvider, LoggerConfiguration loggerConfiguration)
    {
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var options = serviceProvider.GetRequiredService<IOptions<SerilogBuilderOptions>>().Value;

        // enrich the log events appropriately
        loggerConfiguration.Enrich.FromLogContext();
        loggerConfiguration.Enrich.With(new EnvironmentEnricher(environment));
        loggerConfiguration.Enrich.WithSensitiveDataMasking(opt =>
        {
            opt.ExcludeProperties.AddRange(new[]
            {
                "ApplicationVersion", // may contain Git SHA
                "MachineName", // may contain SHA
            });

            var configureSensitiveDataEnricherOptions = Merge(options.ConfigureSensitiveDataEnricherOptions);
            configureSensitiveDataEnricherOptions?.Invoke(opt);
        });

        // add destructuring of exception
        var destructuringOptions = new DestructuringOptionsBuilder()
            .WithDefaultDestructurers()
            .WithIgnoreStackTraceAndTargetSiteExceptionFilter();
        var configureDestructuringOptions = Merge(options.ConfigureDestructuringOptions);
        configureDestructuringOptions?.Invoke(destructuringOptions);
        loggerConfiguration.Enrich.WithExceptionDetails(destructuringOptions);

        // read the configuration data
        loggerConfiguration.ReadFrom.Configuration(configuration);
        loggerConfiguration.ReadFrom.Settings(new ConvertedSerilogSettings(configuration.GetSection("Logging")));

        // write to debug
        loggerConfiguration.WriteTo.Debug(restrictedToMinimumLevel: configuration.GetDefaultEventLevelForProvider("Debug"));

        // write to console
        var consoleJson = bool.TryParse(configuration["Logging:Console:SerilogJsonFormat"], out var b) && b;
        if (consoleJson)
        {
            loggerConfiguration.WriteTo.Console(formatter: new CompactJsonFormatter(),
                                                restrictedToMinimumLevel: configuration.GetDefaultEventLevelForProvider("Console"));
        }
        else
        {
            loggerConfiguration.WriteTo.Console(restrictedToMinimumLevel: configuration.GetDefaultEventLevelForProvider("Console"),
                                                theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code);
        }

        // write to Seq if configured
        var serverUrl = configuration["Logging:Seq:ServerUrl"] ?? configuration["Seq:ServerUrl"];
        if (serverUrl is not null)
        {
            var apiKey = configuration["Logging:Seq:ApiKey"] ?? configuration["Seq:ApiKey"];
            loggerConfiguration.WriteTo.Seq(serverUrl: serverUrl,
                                            restrictedToMinimumLevel: configuration.GetDefaultEventLevelForProvider("Seq"),
                                            apiKey: apiKey);
        }

        // allow further configuration of the loggerConfiguration
        var configureLoggerConfiguration = Merge(options.ConfigureLoggerConfiguration);
        configureLoggerConfiguration?.Invoke(loggerConfiguration);
    }

    internal static Action<T>? Merge<T>(IReadOnlyCollection<Action<T>> actions)
        => actions is null || actions.Count <= 0 ? null : actions.Aggregate((a, b) => a + b);
}
