using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Tingle.Extensions.Logging.LogAnalytics;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Logging extensions for Azure LogAnalytics
    /// </summary>
    public static class ILoggingBuilderExtensions
    {
        /// <summary>
        /// Adds a LogAnalytics logger named 'LogAnalytics' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configureOptions">Action to configure LogAnalytics logger.</param>
        public static ILoggingBuilder AddLogAnalytics(this ILoggingBuilder builder,
                                                      Action<LogAnalyticsLoggerOptions> configureOptions)
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LogAnalyticsLoggerProvider>());
            builder.Services.Configure(configureOptions);

            return builder;
        }

        /// <summary>
        /// Adds a LogAnalytics logger named 'LogAnalytics' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configuration">The configuration that contains values for <see cref="LogAnalyticsLoggerOptions"/>.</param>
        public static ILoggingBuilder AddLogAnalytics(this ILoggingBuilder builder, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return builder.AddLogAnalytics((options) =>
            {
                options.WorkspaceId = configuration.GetValue<string>(nameof(options.WorkspaceId));
                options.WorkspaceKey = configuration.GetValue<string>(nameof(options.WorkspaceKey));
                options.LogTypeName = configuration.GetValue<string>(nameof(options.LogTypeName)) ?? Constants.DefaultLogTypeName;
            });
        }

        /// <summary>
        /// Adds a LogAnalytics logger named 'LogAnalytics' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="workspaceId">The unique identifier of the LogAnalytics workspace.</param>
        /// <param name="workspaceKey">The key for authentication against the LogAnalytics endpoint.</param>
        /// <param name="logTypeName">
        /// The name that appears appear on LogAnalytics navigation menu.
        /// This value is mainly used for ease of categorization of logs from different applications.
        /// Each service/application should ideally have its own if the logs are meant to be separate.
        /// Defaults to <see cref="Constants.DefaultLogTypeName"/>
        /// </param>
        public static ILoggingBuilder AddLogAnalytics(this ILoggingBuilder builder,
                                                      string workspaceId,
                                                      string workspaceKey,
                                                      string logTypeName = Constants.DefaultLogTypeName)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddLogAnalytics((options) =>
            {
                options.WorkspaceId = workspaceId;
                options.WorkspaceKey = workspaceKey;
                options.LogTypeName = logTypeName;
            });
        }
    }
}
