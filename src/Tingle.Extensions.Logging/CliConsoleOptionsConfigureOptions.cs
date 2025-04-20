using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;

namespace Tingle.Extensions.Logging;

/// <summary>
/// Configures a ConsoleFormatterOptions object from an IConfiguration.
/// </summary>
/// <remarks>
/// Doesn't use ConfigurationBinder in order to allow ConfigurationBinder, and all its dependencies,
/// to be trimmed. This improves app size and startup.
/// </remarks>
[UnsupportedOSPlatform("browser")]
internal sealed class CliConsoleOptionsConfigureOptions(ILoggerProviderConfiguration<ConsoleLoggerProvider> provider)
    : IConfigureOptions<CliConsoleOptions>
{
    private readonly IConfiguration configuration = provider.GetFormatterOptionsSection();

    public void Configure(CliConsoleOptions options)
    {
        SetValue<LoggerColorBehavior>(nameof(options.ColorBehavior), v => options.ColorBehavior = v);
        SetValue(nameof(options.IncludeCategory), v => options.IncludeCategory = v);
        SetValue(nameof(options.IncludeEventId), v => options.IncludeEventId = v);
        SetValue(nameof(options.IncludeScopes), v => options.IncludeScopes = v);
        SetValue(nameof(options.IncludeScopes), v => options.IncludeScopes = v);
        SetValue(nameof(options.SingleLine), v => options.SingleLine = v);
        SetValue(nameof(options.TimestampFormat), v => options.TimestampFormat = v);
        SetValue(nameof(options.UseUtcTimestamp), v => options.UseUtcTimestamp = v);
    }

    private void SetValue<T>(string key, Action<T> setter) where T : struct, Enum
    {
        if (Enum.TryParse<T>(configuration[key], ignoreCase: true, out var value)) setter(value);
    }

    private void SetValue(string key, Action<bool> setter)
    {
        if (bool.TryParse(configuration[key], out var value)) setter(value);
    }

    private void SetValue(string key, Action<string?> setter) => setter(configuration[key]);
}