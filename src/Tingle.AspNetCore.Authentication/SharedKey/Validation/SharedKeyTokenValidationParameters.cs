using Microsoft.AspNetCore.Http;

namespace Tingle.AspNetCore.Authentication.SharedKey.Validation;

/// <summary>
/// Validation parameters to use with <see cref="ISharedKeyTokenValidator"/>
/// </summary>
public class SharedKeyTokenValidationParameters
{
    /// <summary>
    /// The list of known api key values. These values are added to the results of
    /// <see cref="KeysResolver"/> when checking for correct values.
    /// </summary>
    public ICollection<string> KnownFixedKeys { get; set; } = new HashSet<string>();

    /// <summary>
    /// The name of the headers from which we can extract a date that must be supplied in each incoming request
    /// </summary>
    public ICollection<string> DateHeaderNames { get; set; } = new HashSet<string> { SharedKeyDefaults.DateHeaderName, "x-ms-date", };

    /// <summary>
    /// The prefix to be applied to the path before computing signature for comparison.
    /// Useful if the application is hosted in a folder e.g. '/app1'.
    /// The value must begin with a '/'.
    /// Leave null where not in use
    /// </summary>
    public string? PathPrefix { get; set; }

    /// <summary>
    /// The amount of time allowed between now and the time the request was made as indicated in the date header
    /// whose possible names are specified using <see cref="DateHeaderNames"/>.
    /// Set to null to disable time allowance. Defaults to 5min
    /// </summary>
    public TimeSpan? TimeAllowance { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Resolver for signing keys
    /// </summary>
    public Func<HttpContext, Task<IEnumerable<string>>> KeysResolver { get; set; } = (ctx) => Task.FromResult<IEnumerable<string>>([]);

    internal async Task<IEnumerable<string>> ResolveKeysAsync(HttpContext httpContext)
    {
        var keys = (await KeysResolver(httpContext).ConfigureAwait(false) ?? []).ToList();
        keys.AddRange(KnownFixedKeys);
        return keys;
    }
}
