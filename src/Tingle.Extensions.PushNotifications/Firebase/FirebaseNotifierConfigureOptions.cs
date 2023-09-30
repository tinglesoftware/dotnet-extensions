using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

internal class FirebaseNotifierConfigureOptions : IValidateOptions<FirebaseNotifierOptions>
{
    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, FirebaseNotifierOptions options)
    {
        // ensure we have a ProjectId
        if (string.IsNullOrEmpty(options.ProjectId))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.ProjectId)} must be provided");
        }

        // ensure we have a ClientEmail
        if (string.IsNullOrEmpty(options.ClientEmail))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.ClientEmail)} must be provided");
        }

        // ensure we have a TokenUri
        if (string.IsNullOrEmpty(options.TokenUri))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.TokenUri)} must be provided");
        }

        // ensure we have a PrivateKey
        if (string.IsNullOrEmpty(options.PrivateKey))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.PrivateKey)} must be provided");
        }

        return ValidateOptionsResult.Success;
    }
}
