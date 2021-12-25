using System.Runtime.Serialization;

namespace Tingle.Extensions.Logging.LogAnalytics;

internal class LogAnalyticsLoggerException : Exception
{
    public LogAnalyticsLoggerException(string message) : base(message) { }

    public LogAnalyticsLoggerException(string message, Exception innerException) : base(message, innerException) { }

    public LogAnalyticsLoggerException() { }

    protected LogAnalyticsLoggerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
