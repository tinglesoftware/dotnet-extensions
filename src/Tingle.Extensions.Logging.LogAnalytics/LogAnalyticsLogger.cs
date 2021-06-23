using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Tingle.Extensions.Logging.LogAnalytics
{
    /// <summary>
    /// LogAnalytics logger implementation for <see cref="ILogger"/>.
    /// </summary>
    /// <seealso cref="ILogger" />
    public class LogAnalyticsLogger : ILogger
    {
        private readonly HttpClient httpClient;
        private readonly string categoryName;
        private readonly LogAnalyticsLoggerOptions options;

        /// <summary>
        /// Creates an instance of <see cref="LogAnalyticsLogger"/>
        /// </summary>
        public LogAnalyticsLogger(string categoryName, HttpClient httpClient, LogAnalyticsLoggerOptions options)
        {
            this.categoryName = categoryName;
            this.httpClient = httpClient;
            this.options = options;
        }

        /// <summary>
        /// Gets or sets the external scope provider.
        /// </summary>
        internal IExternalScopeProvider? ExternalScopeProvider { get; set; }

        ///<inheritdoc/>
        public IDisposable BeginScope<TState>(TState state) => ExternalScopeProvider?.Push(state) ?? NullScope.Instance;

        ///<inheritdoc/>
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        ///<inheritdoc/>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // ensure the formatter is not null
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            try
            {
                if (IsEnabled(logLevel))
                {
                    // Create the message
                    var message = formatter(state, exception);

                    // Prepare the data to log
                    var data = new Dictionary<string, object>
                    {
                        ["Message"] = message,
                        ["CategoryName"] = categoryName,
                        ["LogLevel"] = logLevel.ToString()
                    };

                    // Populate the event information
                    if (eventId.Id != 0) data["EventId"] = eventId.Id;
                    if (!string.IsNullOrEmpty(eventId.Name)) data["EventName"] = eventId.Name;

                    // Populate the exception information if there's one
                    if (exception != null)
                    {
                        data["Exception"] = new Dictionary<string, string>
                        {
                            ["Message"] = exception.Message,
                            ["StackTrace"] = exception.StackTrace,
                            ["Source"] = exception.Source,
                        };
                    }

                    // Populate the state and scope information
                    if (options.IncludeScopes)
                    {
                        // Populate the state information
                        if (state is IReadOnlyCollection<KeyValuePair<string, object>> stateDictionary)
                        {
                            foreach (KeyValuePair<string, object> item in stateDictionary)
                            {
                                data[item.Key] = Convert.ToString(item.Value, CultureInfo.InvariantCulture);
                            }
                        }

                        // Populate the scope information
                        if (ExternalScopeProvider != null)
                        {
                            var stringBuilder = new StringBuilder();
                            ExternalScopeProvider.ForEachScope(
                                callback: (activeScope, builder) =>
                                {
                                    // Ideally we expect that the scope to implement IReadOnlyList<KeyValuePair<string, object>>.
                                    // But this is not guaranteed as user can call BeginScope and pass anything. Hence
                                    // we try to resolve the scope as Dictionary and if we fail, we just serialize the object and add it.

                                    if (activeScope is IReadOnlyCollection<KeyValuePair<string, object>> activeScopeDictionary)
                                    {
                                        foreach (KeyValuePair<string, object> item in activeScopeDictionary)
                                        {
                                            data[item.Key] = Convert.ToString(item.Value, CultureInfo.InvariantCulture);
                                        }
                                    }
                                    else
                                    {
                                        builder.Append(" => ").Append(activeScope);
                                    }
                                },
                                state: stringBuilder);

                            if (stringBuilder.Length > 0)
                            {
                                data["Scope"] = stringBuilder.ToString();
                            }
                        }
                    }

                    // Create payload in JSON and key from Base64
                    var payload = System.Text.Json.JsonSerializer.Serialize(data);
                    var key = Convert.FromBase64String(options.WorkspaceKey);
                    var tsk = httpClient.UploadAsync(workspaceId: options.WorkspaceId!,
                                                     workspaceKey: key,
                                                     payload: payload,
                                                     logType: options.LogTypeName,
                                                     generated: null);

                    tsk.ConfigureAwait(false); // reasons are well known

                    // NOTE: the task is not awaited and is not synchronously called.
                    // Doing so may create a deadlock or cause the caller to be delayed.
                }
            }
            catch (Exception ex)
            {
                LogAnalyticsLoggerEventSource.Log.FailedToLog(ex.ToInvariantString());
            }
        }
    }
}
