using Microsoft.Extensions.Options;

namespace Tingle.AspNetCore.Authentication.PassThrough;

/// <summary>
/// PassThrough post configure options
/// </summary>
internal class PassThroughPostConfigureOptions : IPostConfigureOptions<PassThroughOptions>
{
    public void PostConfigure(string? name, PassThroughOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.Events ??= new PassThroughEvents();
    }
}
