using System.Diagnostics.Tracing;
using System.Reflection;

namespace Tingle.Extensions.Logging.LogAnalytics
{
    /// <summary>
    /// EventSource for reporting errors and warnings from Logging module.
    /// </summary>
    [EventSource(Name = "Tingle-LogAnalytics-LoggerProvider")]
    internal class LogAnalyticsLoggerEventSource : EventSource
    {
        public static readonly LogAnalyticsLoggerEventSource Log = new LogAnalyticsLoggerEventSource();
        public readonly string ApplicationName;

        private LogAnalyticsLoggerEventSource()
        {
            ApplicationName = GetApplicationName();
        }

        [Event(1, Message = "Sending log to LogAnalyticsLoggerProvider has failed. Error: {0}", Level = EventLevel.Error)]
        public void FailedToLog(string error, string applicationName = null) => WriteEvent(1, error, applicationName ?? ApplicationName);

        [NonEvent]
        private static string GetApplicationName()
        {
            try
            {
                return Assembly.GetEntryAssembly().GetName().Name;
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
