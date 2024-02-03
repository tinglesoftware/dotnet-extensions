using Microsoft.Extensions.Options;
using Tingle.Extensions.PushNotifications;

namespace Microsoft.Extensions.DependencyInjection;

[Obsolete(MessageStrings.FirebaseLegacyObsoleteMessage)]
internal class FcmLegacyNotifierConfigureOptions : IValidateOptions<FcmLegacyNotifierOptions>
{
    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, FcmLegacyNotifierOptions options)
    {
        // ensure we have a key
        if (string.IsNullOrEmpty(options.Key))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.Key)} must be provided");
        }

        return ValidateOptionsResult.Success;
    }
}
