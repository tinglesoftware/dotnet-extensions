using Microsoft.Extensions.Options;

namespace Tingle.AspNetCore.Authentication.PassThrough;

/// <summary>
/// PassThrough post configure options
/// </summary>
internal class PassThroughPostConfigureOptions : IPostConfigureOptions<PassThroughOptions>
{
    public void PostConfigure(string? name, PassThroughOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        options.Events ??= new PassThroughEvents();
    }
}
