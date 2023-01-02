using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Security.Cryptography;

namespace Tingle.AspNetCore.Tokens.Protection;

/// <summary>
/// Default implementation for <see cref="ITokenProtector{T}"/>
/// </summary>
/// <typeparam name="T">The type on which to perform operations.</typeparam>
internal class TokenProtector<T> : ITokenProtector<T>
{
    private readonly IDataProtector protector;
    private readonly ITimeLimitedDataProtector timeLimitedProtector;
    private readonly TokenProtectorOptions options;

    /// <summary>Initializes a new instance of the <see cref="TokenProtector{T}"/> class.</summary>
    /// <param name="protectionProvider">The application's provider of instances of <see cref="IDataProtector"/>.</param>
    /// <param name="optionsAccessor"></param>
    public TokenProtector(IDataProtectionProvider protectionProvider, IOptionsSnapshot<TokenProtectorOptions> optionsAccessor)
    {
        if (protectionProvider == null) throw new ArgumentNullException(nameof(protectionProvider));

        protector = protectionProvider.CreateProtector(TokenDefaults.ProtectorPurpose);

        // ToTimeLimitedDataProtector() creates a wrapper around the protector but does not initialize it until
        // Protect/UnProtect with time-based arguments is called.
        // There is, therefore, no need to protect its initialization
        timeLimitedProtector = protector.ToTimeLimitedDataProtector();

        options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
    }

    /// <summary>
    /// Decrypts a string to a token's value.
    /// </summary>
    /// <param name="encrypted">The Base64-encoded value to decrypt.</param>
    /// <returns>The decrypted value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encrypted"/> is <see langword="null"/>.</exception>
    /// <exception cref="CryptographicException">The decryption operation has failed.</exception>
    public virtual T? UnProtect(string encrypted)
    {
        if (encrypted == null) throw new ArgumentNullException(nameof(encrypted));

        var raw = protector.Unprotect(encrypted);
        return Deserialize(raw);
    }

    /// <summary>
    /// Decrypts a string to a token's value.
    /// </summary>
    /// <param name="encrypted">The Base64-encoded value to decrypt.</param>
    /// <param name="expiration">
    /// An 'out' parameter which upon a successful unprotect operation receives the expiration date of the payload.
    /// </param>
    /// <returns>The decrypted value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encrypted"/> is <see langword="null"/>.</exception>
    /// <exception cref="CryptographicException">The decryption operation has failed.</exception>
    public virtual T? UnProtect(string encrypted, out DateTimeOffset expiration)
    {
        if (encrypted == null) throw new ArgumentNullException(nameof(encrypted));

        var raw = timeLimitedProtector.Unprotect(encrypted, out expiration);
        return Deserialize(raw);
    }

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    public virtual string Protect(T value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        var raw = Serialize(value);
        return protector.Protect(raw);
    }

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="expiration">The time when this payload should expire.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    public virtual string Protect(T value, DateTimeOffset expiration)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        var raw = Serialize(value);
        return timeLimitedProtector.Protect(raw, expiration);
    }

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="lifetime">The lifespan of the protected payload.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    public virtual string Protect(T value, TimeSpan lifetime)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        var raw = Serialize(value);
        return timeLimitedProtector.Protect(raw, lifetime);
    }

    protected virtual T? Deserialize(string plaintext)
    {
        return options.UseConversionInsteadOfJson
            ? (T?)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(plaintext)
            : System.Text.Json.JsonSerializer.Deserialize<T>(plaintext);
    }

    protected virtual string Serialize(T value)
    {
        return options.UseConversionInsteadOfJson
            ? TypeDescriptor.GetConverter(typeof(T)).ConvertToInvariantString(value)!
            : System.Text.Json.JsonSerializer.Serialize(value);
    }
}
