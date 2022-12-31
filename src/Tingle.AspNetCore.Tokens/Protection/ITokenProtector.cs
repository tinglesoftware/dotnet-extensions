using System.Security.Cryptography;

namespace Tingle.AspNetCore.Tokens.Protection;

/// <summary>
/// Provides symmetric encryption and decryption utilities for tokens
/// </summary>
/// <typeparam name="T">The type on which to perform operations.</typeparam>
public interface ITokenProtector<T>
{
    /// <summary>
    /// Decrypts a string to a token's value.
    /// </summary>
    /// <param name="encrypted">The Base64-encoded value to decrypt.</param>
    /// <returns>The decrypted value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encrypted"/> is <see langword="null"/>.</exception>
    /// <exception cref="CryptographicException">The decryption operation has failed.</exception>
    T? UnProtect(string encrypted);

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
    T? UnProtect(string encrypted, out DateTimeOffset expiration);

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    string Protect(T value);

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="expiration">The time when this payload should expire.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    string Protect(T value, DateTimeOffset expiration);

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="lifetime">The lifespan of the protected payload.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    string Protect(T value, TimeSpan lifetime);
}
