using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

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
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>The decrypted value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encrypted"/> is <see langword="null"/>.</exception>
    /// <exception cref="CryptographicException">The decryption operation has failed.</exception>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    T? UnProtect(string encrypted, JsonSerializerOptions? options = null);

    /// <summary>
    /// Decrypts a string to a token's value.
    /// </summary>
    /// <param name="encrypted">The Base64-encoded value to decrypt.</param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <returns>The decrypted value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encrypted"/> is <see langword="null"/>.</exception>
    /// <exception cref="CryptographicException">The decryption operation has failed.</exception>
    T? UnProtect(string encrypted, JsonTypeInfo<T> jsonTypeInfo);

    /// <summary>
    /// Decrypts a string to a token's value.
    /// </summary>
    /// <param name="encrypted">The Base64-encoded value to decrypt.</param>
    /// <param name="expiration">
    /// An 'out' parameter which upon a successful unprotect operation receives the expiration date of the payload.
    /// </param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>The decrypted value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encrypted"/> is <see langword="null"/>.</exception>
    /// <exception cref="CryptographicException">The decryption operation has failed.</exception>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    T? UnProtect(string encrypted, out DateTimeOffset expiration, JsonSerializerOptions? options = null);

    /// <summary>
    /// Decrypts a string to a token's value.
    /// </summary>
    /// <param name="encrypted">The Base64-encoded value to decrypt.</param>
    /// <param name="expiration">
    /// An 'out' parameter which upon a successful unprotect operation receives the expiration date of the payload.
    /// </param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <returns>The decrypted value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encrypted"/> is <see langword="null"/>.</exception>
    /// <exception cref="CryptographicException">The decryption operation has failed.</exception>
    T? UnProtect(string encrypted, out DateTimeOffset expiration, JsonTypeInfo<T> jsonTypeInfo);

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="options">Options to control serialization behavior.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    string Protect(T value, JsonSerializerOptions? options = null);

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    string Protect(T value, JsonTypeInfo<T> jsonTypeInfo);

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="expiration">The time when this payload should expire.</param>
    /// <param name="options">Options to control serialization behavior.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    string Protect(T value, DateTimeOffset expiration, JsonSerializerOptions? options = null);

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="expiration">The time when this payload should expire.</param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    string Protect(T value, DateTimeOffset expiration, JsonTypeInfo<T> jsonTypeInfo);

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="lifetime">The lifespan of the protected payload.</param>
    /// <param name="options">Options to control serialization behavior.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    string Protect(T value, TimeSpan lifetime, JsonSerializerOptions? options = null);

    /// <summary>
    /// Encrypts a token's value to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="lifetime">The lifespan of the protected payload.</param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <returns>The Base64-encoded encrypted value.</returns>
    string Protect(T value, TimeSpan lifetime, JsonTypeInfo<T> jsonTypeInfo);
}
