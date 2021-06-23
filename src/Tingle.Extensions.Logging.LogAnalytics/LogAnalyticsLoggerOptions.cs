namespace Tingle.Extensions.Logging.LogAnalytics
{
    /// <summary>
    /// <see cref="LogAnalyticsLoggerOptions"/> defines the custom behavior of the data sent to Log Analytics.
    /// </summary>
    public class LogAnalyticsLoggerOptions
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LogAnalytics workspace.
        /// </summary>
        public string? WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the key for authentication against the LogAnalytics endpoint.
        /// </summary>
        public string? WorkspaceKey { get; set; }

        /// <summary>
        /// Gets or sets the name that appears appear on LogAnalytics navigation menu.
        /// This value is mainly used for ease of categorization of logs from different applications.
        /// Each service/application should ideally have its own if the logs are meant to be separate.
        /// Defaults to <see cref="Constants.DefaultLogTypeName"/>
        /// </summary>
        public string LogTypeName { get; set; } = Constants.DefaultLogTypeName;

        /// <summary>
        /// Gets or sets a value indicating whether the Scope information is included from telemetry or not.
        /// Defaults to true.
        /// </summary>
        public bool IncludeScopes { get; set; } = true;
    }
}
