using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace Tingle.Extensions.Logging.LogAnalytics;

/// <summary>
/// Represents a type that can create instances of <see cref="LogAnalyticsLogger"/>.
/// </summary>
[ProviderAlias("LogAnalytics")]
public sealed class LogAnalyticsLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    // The client to be used to log messages to LogAnalytics.
    private readonly HttpClient httpClient;
    // The LogAnalytics logger options.
    private readonly LogAnalyticsLoggerOptions options;
    // The external scope provider to allow setting scope data in messages.
    private IExternalScopeProvider? externalScopeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogAnalyticsLoggerProvider"/> class.
    /// </summary>
    /// <param name="optionsAccessor">The application insights logger options.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="optionsAccessor"/> or it's value is null .
    /// </exception>
    public LogAnalyticsLoggerProvider(IOptions<LogAnalyticsLoggerOptions> optionsAccessor)
    {
        options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
        httpClient = new HttpClient();
    }

    ///<inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        return new LogAnalyticsLogger(categoryName, httpClient, options)
        {
            ExternalScopeProvider = externalScopeProvider,
        };
    }

    ///<inheritdoc/>
    public void Dispose() => httpClient.Dispose();

    /// <summary>
    /// Sets the scope provider. This method also updates all the existing logger to also use the new ScopeProvider.
    /// </summary>
    /// <param name="externalScopeProvider">The external scope provider.</param>
    public void SetScopeProvider(IExternalScopeProvider externalScopeProvider)
    {
        this.externalScopeProvider = externalScopeProvider;
    }
}
