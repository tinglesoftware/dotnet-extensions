using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Microsoft.Extensions.Configuration;

internal static class ConfigurationExtensions
{
    public static LogEventLevel GetDefaultEventLevelForProvider(this IConfiguration configuration, string providerName)
    {
        return Enum.TryParse<LogLevel>(configuration[$"Logging:{providerName}:LogLevel:Default"], ignoreCase: true, out var parsed)
            ? parsed.ToLogEventLevel()
            : LogEventLevel.Verbose;
    }
}
