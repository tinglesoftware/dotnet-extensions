using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

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
