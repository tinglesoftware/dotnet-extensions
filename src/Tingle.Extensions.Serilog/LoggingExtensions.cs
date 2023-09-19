using Serilog.Events;

namespace Microsoft.Extensions.Logging;

internal static class LoggingExtensions
{
    public static LogEventLevel ToLogEventLevel(this LogLevel level)
    {
        // https://github.com/serilog/serilog-extensions-logging/blob/e25ed7ddfd0c664bd3a7e9cdbeeec6f87ff12964/src/Serilog.Extensions.Logging/Extensions/Logging/LevelConvert.cs#L35-L54
        return level switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            LogLevel.None => LogEventLevel.Fatal,
            _ => LogEventLevel.Verbose,
        };
    }
}
