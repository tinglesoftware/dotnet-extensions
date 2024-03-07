using System.Reflection;
using Tingle.Extensions.Http;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IHttpClientBuilder"/>.
/// </summary>
public static class IHttpClientBuilderExtensions
{
    /// <summary>
    /// Adds a <see cref="DelegatingHandler"/> that adds a <c>User-Agent</c> header to each outgoing request.
    /// The version is pulled from the assembly containing <typeparamref name="T"/>
    /// whereas the name is pulled from the entry assembly or the one containing <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type from which to pull the assembly version.</typeparam>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/> to use.</param>
    /// <param name="clear">Whether to clear <c>User-Agent</c> headers.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/>.</returns>
    public static IHttpClientBuilder AddUserAgentVersionHandler<T>(this IHttpClientBuilder builder, bool clear = false)
    {
        var name = Assembly.GetEntryAssembly()?.GetName().Name
                ?? typeof(T).Assembly.GetName().Name
                ?? throw new InvalidOperationException("Unable to get the name from the entry assembly or the one containing the type argument.");
        return builder.AddUserAgentVersionHandler<T>(name, clear);
    }

    /// <summary>
    /// Adds a <see cref="DelegatingHandler"/> that adds a <c>User-Agent</c> header to each outgoing request.
    /// The version is pulled from the assembly containing <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type from which to pull the assembly version.</typeparam>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/> to use.</param>
    /// <param name="name">The product name to use.</param>
    /// <param name="clear">Whether to clear <c>User-Agent</c> headers.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/>.</returns>
    public static IHttpClientBuilder AddUserAgentVersionHandler<T>(this IHttpClientBuilder builder, string name, bool clear = false)
    {
        return builder.AddUserAgentVersionHandler(typeof(T), name, clear);
    }

    /// <summary>
    /// Adds a <see cref="DelegatingHandler"/> that adds a <c>User-Agent</c> header to each outgoing request.
    /// The version is pulled from the assembly containing <paramref name="type"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/> to use.</param>
    /// <param name="type">The type from which to pull the assembly version</param>
    /// <param name="name">The product name to use.</param>
    /// <param name="clear">Whether to clear <c>User-Agent</c> headers.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/>.</returns>
    public static IHttpClientBuilder AddUserAgentVersionHandler(this IHttpClientBuilder builder, Type type, string name, bool clear = false)
    {
        return builder.AddUserAgentVersionHandler(type.Assembly, name, clear);
    }

    /// <summary>
    /// Adds a <see cref="DelegatingHandler"/> that adds a <c>User-Agent</c> header to each outgoing request.
    /// The version is pulled from the <paramref name="assembly"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/> to use.</param>
    /// <param name="assembly">The <see cref="Assembly"/> from which to pull the version.</param>
    /// <param name="name">The product name to use.</param>
    /// <param name="clear">Whether to clear <c>User-Agent</c> headers.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/>.</returns>
    public static IHttpClientBuilder AddUserAgentVersionHandler(this IHttpClientBuilder builder, Assembly assembly, string name, bool clear = false)
    {
        /*
         * Use the informational version if available because it has the git commit sha.
         * Using the git commit sha allows for maximum reproduction.
         * 
         * Examples:
         * 1) 1.7.1-ci.131+Branch.main.Sha.752f6cdfabb76e65d2b2cd18b3b284ef65713213
         * 2) 1.7.1-PullRequest10247.146+Branch.pull-10247-merge.Sha.bf46008b75eacacad3b7654959d38f8df4c7fcdb
         * 3) 1.7.1-fixes-2021-10-12-2.164+Branch.fixes-2021-10-12-2.Sha.bf46008b75eacacad3b7654959d38f8df4c7fcdb
         * 4) 1.9.3+Branch.migration-to-hc.Sha.ed9934bab03eaca1dfcef2c212372f1e6820418e
         * 
         * When not available, use the usual assembly version
         */
        string? version = null;
        var attr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attr is not null && !string.IsNullOrWhiteSpace(attr.InformationalVersion))
        {
            version = attr.InformationalVersion;
        }
        else
        {
            version ??= assembly.GetName().Version!.ToString(3);
        }

        return builder.AddUserAgentVersionHandler(name, version, clear);
    }

    /// <summary>
    /// Adds a <see cref="DelegatingHandler"/> that adds a <c>User-Agent</c> header to each outgoing request.
    /// </summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/> to use.</param>
    /// <param name="name">The product name to use.</param>
    /// <param name="version">The version to use.</param>
    /// <param name="clear">Whether to clear <c>User-Agent</c> headers.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/>.</returns>
    public static IHttpClientBuilder AddUserAgentVersionHandler(this IHttpClientBuilder builder, string name, string version, bool clear = false)
    {
        if (builder is null) throw new ArgumentNullException(nameof(builder));
        if (string.IsNullOrEmpty(name)) throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrEmpty(version)) throw new ArgumentException($"'{nameof(version)}' cannot be null or empty.", nameof(version));

        // a message handler is used because, some scenarios do not support configuring HttpClient (e.g. gRPC clients via DI)
        return builder.AddHttpMessageHandler(() => new UserAgentVersionHandler(name, version, clear));
    }
}
