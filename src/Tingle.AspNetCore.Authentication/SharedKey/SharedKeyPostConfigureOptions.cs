using Microsoft.Extensions.Options;

namespace Tingle.AspNetCore.Authentication.SharedKey;

/// <summary>
/// SharedKey post configure options
/// </summary>
internal class SharedKeyPostConfigureOptions : IPostConfigureOptions<SharedKeyOptions>
{
    public void PostConfigure(string? name, SharedKeyOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        options.Events ??= new SharedKeyEvents();

        if (string.IsNullOrWhiteSpace(options.HeaderValuePrefix))
            throw new ArgumentNullException(nameof(options.HeaderValuePrefix));

        if (options.ValidationParameters.DateHeaderNames == null || options.ValidationParameters.DateHeaderNames.Count < 1)
        {
            throw new InvalidOperationException($"{nameof(options.ValidationParameters.DateHeaderNames)} must have at least one value");
        }

        if (options.ValidationParameters.KeysResolver == null)
        {
            options.ValidationParameters.KeysResolver = (c) => Task.FromResult<IEnumerable<string>>([]);
        }

        // if path prefix is specified, it must start with a '/'
        if (!string.IsNullOrWhiteSpace(options.ValidationParameters.PathPrefix) && !options.ValidationParameters.PathPrefix.StartsWith("/"))
        {
            throw new ArgumentException($"{nameof(options.ValidationParameters.PathPrefix)} must start with '/' or be null",
                                        nameof(options.ValidationParameters.PathPrefix));
        }
    }
}
