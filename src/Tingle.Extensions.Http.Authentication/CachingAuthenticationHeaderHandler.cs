using AnyOfTypes;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Tingle.Extensions.Http.Authentication;

/// <summary>
/// Authentication provider that caches the authentiaction parameter via <see cref="Cache"/>.
/// </summary>
public abstract class CachingAuthenticationHeaderHandler : AuthenticationHeaderHandler
{
    /// <summary>
    /// Creates a new instance of the <see cref="CachingAuthenticationHeaderHandler"/> class.
    /// </summary>
    protected CachingAuthenticationHeaderHandler() { }

    /// <summary>
    /// Creates a new instance of the <see cref="CachingAuthenticationHeaderHandler"/> class with a specific inner handler.
    /// </summary>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
    protected CachingAuthenticationHeaderHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    /// <summary>
    /// The cache object. Set this to null to disable caching.
    /// </summary>
    public virtual AnyOf<IMemoryCache, IDistributedCache>? Cache { get; set; }

    /// <summary>
    /// The key to use storing and retrieving the cached token. It must be unique in the provided cache.
    /// Set this to null to disable caching.
    /// </summary>
    public virtual string? CacheKey { get; set; }

    /// <summary>
    /// The logger used to record events
    /// </summary>
    public virtual ILogger? Logger { get; set; }

    /// <summary>
    /// Gets the token stored in cache
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected async Task<string?> GetTokenFromCacheAsync(CancellationToken cancellationToken)
    {
        if (TryGetCache(out var cache, out var key))
        {
            return cache switch
            {
                IMemoryCache memoryCache => memoryCache.Get<string>(key),
                IDistributedCache distributedCache => await distributedCache.GetStringAsync(key, cancellationToken).ConfigureAwait(false),
                _ => null,
            };
        }

        return null;
    }

    /// <summary>
    /// Saves the token in the cache
    /// </summary>
    /// <param name="value"></param>
    /// <param name="expiresOn"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected async Task SetTokenInCacheAsync(string value, DateTimeOffset expiresOn, CancellationToken cancellationToken)
    {
        if (TryGetCache(out var cache, out var key))
        {
            if (cache is IMemoryCache memoryCache)
            {
                var options = new MemoryCacheEntryOptions { AbsoluteExpiration = expiresOn };
                memoryCache.Set(key, value, options);
            }
            else if (cache is IDistributedCache distributedCache)
            {
                var options = new DistributedCacheEntryOptions { AbsoluteExpiration = expiresOn };
                await distributedCache.SetStringAsync(key: key, value: value, options: options, token: cancellationToken).ConfigureAwait(false);
            }
            else throw new NotSupportedException();
        }
    }

    private bool TryGetCache([NotNullWhen(true)] out object? cache, [NotNullWhen(true)] out string? key, bool throwOnUnknownType = true)
    {
        cache = null;

        var value = Cache?.CurrentValue;
        key = CacheKey;
        if (value is null || string.IsNullOrWhiteSpace(key))
        {
            if (cache is not null && string.IsNullOrWhiteSpace(key))
            {
                Logger?.LogWarning("A cache instance is configured without a key. Caching will not work.");
            }

            if (!string.IsNullOrWhiteSpace(key) && cache is null)
            {
                Logger?.LogWarning("A cache key is configured without a cache instance. Caching will not work.");
            }

            return false;
        }

        if (value is not IMemoryCache && value is not IDistributedCache)
        {
            var msg = $"Cache of type: '{value.GetType()}' is not supported";
            Logger?.LogWarning(msg);
            if (throwOnUnknownType)
            {
                throw new NotSupportedException(msg);
            }

            return false;
        }

        cache = value;
        return true;
    }
}
