using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

internal class ApnsNotifierConfigureOptions : IValidateOptions<ApnsNotifierOptions>
{
    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, ApnsNotifierOptions options)
    {
        // ensure we have a BundleId
        if (string.IsNullOrEmpty(options.BundleId))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.BundleId)} must be provided");
        }

        // ensure we have a PrivateKeyBytes resolver
        if (options.PrivateKeyBytes is null)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.PrivateKeyBytes)} must be provided");
        }

        // ensure we have a KeyId
        if (string.IsNullOrEmpty(options.KeyId))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.KeyId)} must be provided");
        }

        // ensure we have a TeamId
        if (string.IsNullOrEmpty(options.TeamId))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.TeamId)} must be provided");
        }

        return ValidateOptionsResult.Success;
    }
}
