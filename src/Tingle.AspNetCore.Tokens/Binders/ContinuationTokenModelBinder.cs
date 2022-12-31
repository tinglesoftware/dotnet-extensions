using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Tingle.AspNetCore.Tokens.Protection;

namespace Tingle.AspNetCore.Tokens.Binders;

/// <summary>
/// Performs model binding for action parameters declared as <see cref="ContinuationToken{T}"/>.
/// </summary>
/// <typeparam name="T">The type of data contained in the token.</typeparam>
internal class ContinuationTokenModelBinder<T> : IModelBinder
{
    private readonly ITokenProtector<T> protector;
    private readonly ILogger logger;

    /// <summary>Initializes a new instance of the <see cref="ContinuationTokenModelBinder{T}"/> class.</summary>
    /// <param name="protector">
    /// The protector for a token. It converts between <typeparamref name="T"/> and <see cref="string"/>.
    /// The implementation is responsible for encrypting and decrypting where needed.
    /// </param>
    /// <param name="logger">The application's logger, specialized for <see cref="ContinuationTokenModelBinder{T}"/>.</param>
    public ContinuationTokenModelBinder(ITokenProtector<T> protector, ILogger<ContinuationTokenModelBinder<T>> logger)
    {
        this.protector = protector ?? throw new ArgumentNullException(nameof(protector));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var name = bindingContext.BinderModelName ?? bindingContext.FieldName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(name);

        // if there is no value from the provider, succeed
        if (valueProviderResult == ValueProviderResult.None)
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(name, valueProviderResult);

        // if there is no actual value set even though the provider got value, succeed
        var encrypted = valueProviderResult.FirstValue;
        if (string.IsNullOrEmpty(encrypted))
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        // at this point, we have a valid string that we can decrypt
        try
        {
            ContinuationToken<T>? token = default;
            if (bindingContext.ModelType == typeof(TimedContinuationToken<T>))
            {
                var decrypted = protector.UnProtect(encrypted, out DateTimeOffset expiration);
                if (decrypted is not null)
                {
                    token = new TimedContinuationToken<T>(decrypted, encrypted, expiration);
                }
            }
            else
            {
                var decrypted = protector.UnProtect(encrypted);
                if (decrypted is not null)
                {
                    token = new ContinuationToken<T>(decrypted, encrypted);
                }
            }

            if (token is not null)
            {
                bindingContext.Result = ModelBindingResult.Success(token);
            }
            else
            {
                logger.LogWarning("Token deserialization failed!");
                bindingContext.ModelState.TryAddModelError(name, "The token is invalid or expired.");
            }
        }
        catch (CryptographicException ce)
        {
            logger.LogInformation(ce, "Token decryption failed!");
            bindingContext.ModelState.TryAddModelError(name, "The token is invalid or expired.");
        }

        return Task.CompletedTask;
    }
}
