namespace Microsoft.Extensions.Logging;

internal static partial class ILoggerExtensions
{
    [LoggerMessage(1, LogLevel.Debug, "Standalone servers (e.g. localhost) do not support transactions. Transactions will be omitted.")]
    public static partial void StandaloneServerNotSupported(this ILogger logger);
}
