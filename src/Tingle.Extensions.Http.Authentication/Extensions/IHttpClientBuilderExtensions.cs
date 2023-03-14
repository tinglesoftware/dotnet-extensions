using Tingle.Extensions.Http.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring an <see cref="IHttpClientBuilder"/>
/// </summary>
public static class IHttpClientBuilderExtensions
{
    #region Generic

    /// <summary>
    /// Adds a delegate that will be used to create an additional message handler for
    /// a named <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <param name="configureHandler">A delegate that is used to create a <see cref="AuthenticationHandler"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddAuthenticationHandler(this IHttpClientBuilder builder, Func<AuthenticationHandler> configureHandler)
    {
        return builder.AddHttpMessageHandler(configureHandler: configureHandler);
    }

    /// <summary>
    /// Adds a delegate that will be used to create an additional message handler for
    /// a named <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <param name="configureHandler">A delegate that is used to create a <see cref="AuthenticationHandler"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddAuthenticationHandler(this IHttpClientBuilder builder,
                                                              Func<IServiceProvider, AuthenticationHandler> configureHandler)
    {
        return builder.AddHttpMessageHandler(configureHandler: configureHandler);
    }

    /// <summary>
    /// Adds an additional message handler from the dependency injection container for
    /// a named <see cref="HttpClient"/>.
    /// </summary>
    /// <typeparam name="THandler">
    /// The type of the <see cref="AuthenticationHandler"/>. The handler type must be registered
    /// as a transient service.
    /// </typeparam>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddAuthenticationHandler<THandler>(this IHttpClientBuilder builder)
        where THandler : AuthenticationHandler
    {
        return builder.AddHttpMessageHandler<THandler>();
    }

    #endregion

    #region ApiKey (Query/Header)

    /// <summary>Adds API key authentication handler.</summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <param name="authenticationValue">The value to be used for authentication</param>
    /// <param name="queryParameterName">The query parameter name to be used</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddApiKeyQueryAuthenticationHandler(this IHttpClientBuilder builder,
                                                                         string authenticationValue,
                                                                         string queryParameterName = "auth")
    {
        if (string.IsNullOrWhiteSpace(authenticationValue))
        {
            throw new ArgumentException($"'{nameof(authenticationValue)}' cannot be null or whitespace.", nameof(authenticationValue));
        }

        if (string.IsNullOrWhiteSpace(queryParameterName))
        {
            throw new ArgumentException($"'{nameof(queryParameterName)}' cannot be null or whitespace.", nameof(queryParameterName));
        }

        return builder.AddAuthenticationHandler(
            () => new ApiKeyQueryAuthenticationHandler(authenticationValue: authenticationValue,
                                                       queryParameterName: queryParameterName));
    }

    /// <summary>Adds API key authentication handler that puts the value in the header.</summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <param name="parameter">The value to be used for authentication.</param>
    /// <param name="scheme">The scheme to use in header.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddApiKeyHeaderAuthenticationHandler(this IHttpClientBuilder builder,
                                                                          string parameter,
                                                                          string scheme = "ApiKey")
    {
        if (string.IsNullOrWhiteSpace(parameter))
        {
            throw new ArgumentException($"'{nameof(parameter)}' cannot be null or whitespace.", nameof(parameter));
        }

        if (string.IsNullOrWhiteSpace(scheme))
        {
            throw new ArgumentException($"'{nameof(scheme)}' cannot be null or whitespace.", nameof(scheme));
        }

        return builder.AddAuthenticationHandler(
            () => new ApiKeyHeaderAuthenticationHandler(parameter) { Scheme = scheme, });
    }

    #endregion

    #region SharedKey

    /// <summary>Adds shared key authentication handler.</summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <param name="key">The base64 encoded pre-shared key (PSK).</param>
    /// <param name="scheme">The scheme to use in header.</param>
    /// <param name="dateHeaderName">The name of the date header..</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddSharedKeyAuthenticationHandler(this IHttpClientBuilder builder,
                                                                       string key,
                                                                       string scheme = "SharedKey",
                                                                       string dateHeaderName = "x-ts-date")
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        return builder.AddAuthenticationHandler(() => new SharedKeyAuthenticationHandler(key: key)
        {
            Scheme = scheme,
            DateHeaderName = dateHeaderName,
        });
    }

    /// <summary>Adds shared key authentication handler.</summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <param name="key">The bytes representing the pre-shared key (PSK)</param>
    /// <param name="scheme">The scheme to use in header.</param>
    /// <param name="dateHeaderName">The name of the date header..</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddSharedKeyAuthenticationHandler(this IHttpClientBuilder builder,
                                                                       byte[] key,
                                                                       string scheme = "SharedKey",
                                                                       string dateHeaderName = "x-ts-date")
    {
        if (key is null) throw new ArgumentNullException(nameof(key));

        return builder.AddAuthenticationHandler(() => new SharedKeyAuthenticationHandler(key: key)
        {
            Scheme = scheme,
            DateHeaderName = dateHeaderName,
        });
    }

    #endregion
}
