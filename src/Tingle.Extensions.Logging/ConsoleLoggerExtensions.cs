using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Tingle.Extensions.Logging;

namespace Microsoft.Extensions.Logging;

/// <summary>
/// Provides extension methods for the <see cref="ILoggingBuilder"/>
/// and <see cref="ILoggerProviderConfiguration{ConsoleLoggerProvider}"/> classes.
/// </summary>
[UnsupportedOSPlatform("browser")]
public static class ConsoleLoggerExtensions
{
    internal const string FormatterName = "cli";

    /// <summary>
    /// Add the CLI console log formatter named 'cli' to the factory with default properties.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    public static ILoggingBuilder AddCliConsole(this ILoggingBuilder builder)
    {
        builder.AddConsoleFormatter<CliConsoleFormatter, CliConsoleOptions, CliConsoleOptionsConfigureOptions>();
        return builder.AddFormatterWithName(FormatterName);
    }

    /// <summary>
    /// Add and configure a console log formatter named 'cli' to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure the <see cref="ConsoleLogger"/> options for the built-in default log formatter.</param>
    public static ILoggingBuilder AddCliConsole(this ILoggingBuilder builder, Action<CliConsoleOptions> configure)
    {
        builder.AddConsoleFormatter<CliConsoleFormatter, CliConsoleOptions, CliConsoleOptionsConfigureOptions>();
        return builder.AddConsoleWithFormatter(FormatterName, configure);
    }

    internal static ILoggingBuilder AddConsoleWithFormatter<TOptions>(this ILoggingBuilder builder, string name, Action<TOptions> configure)
        where TOptions : ConsoleFormatterOptions
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        builder.AddFormatterWithName(name);
        builder.Services.Configure(configure);

        return builder;
    }

    private static ILoggingBuilder AddFormatterWithName(this ILoggingBuilder builder, string name)
        => builder.AddConsole(options => options.FormatterName = name);

    internal static IConfiguration GetFormatterOptionsSection(this ILoggerProviderConfiguration<ConsoleLoggerProvider> provider)
        => provider.Configuration.GetSection("FormatterOptions");

    private static ILoggingBuilder AddConsoleFormatter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TFormatter, TOptions, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TConfigureOptions>(this ILoggingBuilder builder)
        where TOptions : ConsoleFormatterOptions
        where TFormatter : ConsoleFormatter
        where TConfigureOptions : class, IConfigureOptions<TOptions>
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ConsoleFormatter, TFormatter>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TOptions>, TConfigureOptions>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TOptions>, ConsoleLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions>>());

        return builder;
    }

    [UnsupportedOSPlatform("browser")]
    internal sealed class ConsoleLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions>(ILoggerProviderConfiguration<ConsoleLoggerProvider> provider)
        : ConfigurationChangeTokenSource<TOptions>(provider.GetFormatterOptionsSection())
        where TOptions : ConsoleFormatterOptions
        where TFormatter : ConsoleFormatter
    {
    }
}
