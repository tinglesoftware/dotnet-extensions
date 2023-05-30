using Microsoft.Extensions.Logging;

namespace Tingle.AspNetCore.Authentication;

internal static partial class ILoggerExtensions
{
    [LoggerMessage(1, LogLevel.Information, "Failed to validate the token.")]
    public static partial void TokenValidationFailed(this ILogger logger, Exception ex);

    [LoggerMessage(2, LogLevel.Information, "Successfully validated the token.")]
    public static partial void TokenValidationSucceeded(this ILogger logger);

    [LoggerMessage(3, LogLevel.Error, "Exception occurred while processing message.")]
    public static partial void ErrorProcessingMessage(this ILogger logger, Exception ex);
}
