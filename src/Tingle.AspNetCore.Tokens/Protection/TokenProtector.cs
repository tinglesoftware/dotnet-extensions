using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Tingle.AspNetCore.Tokens.Protection;

/// <summary>
/// Default implementation for <see cref="ITokenProtector{T}"/>.
/// <br />
/// <br />
/// This implementation uses JSON for conversions before protection and after unprotection to 
/// avoid loss in sub-second precision for <see cref="DateTimeOffset"/> and <see cref="DateTime"/> types.
/// Using JSON also allows for the protection of complex types and AoT compilation.
/// </summary>
/// <typeparam name="T">The type on which to perform operations.</typeparam>
internal class TokenProtector<T> : ITokenProtector<T>
{
    private readonly IDataProtector protector;
    private readonly ITimeLimitedDataProtector timeLimitedProtector;
    private readonly JsonSerializerOptions serializerOptions;

    /// <summary>Initializes a new instance of the <see cref="TokenProtector{T}"/> class.</summary>
    /// <param name="protectionProvider">The application's provider of instances of <see cref="IDataProtector"/>.</param>
    /// <param name="optionsAccessor">Configured options for MVC JSON usage</param>
    public TokenProtector(IDataProtectionProvider protectionProvider, IOptions<Microsoft.AspNetCore.Mvc.JsonOptions> optionsAccessor)
    {
        ArgumentNullException.ThrowIfNull(protectionProvider);

        protector = protectionProvider.CreateProtector(TokenDefaults.ProtectorPurpose);

        // ToTimeLimitedDataProtector() creates a wrapper around the protector but does not initialize it until
        // Protect/UnProtect with time-based arguments is called.
        // There is, therefore, no need to protect its initialization
        timeLimitedProtector = protector.ToTimeLimitedDataProtector();

        serializerOptions = optionsAccessor.Value.JsonSerializerOptions;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public virtual T? UnProtect(string encrypted, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(encrypted);

        var raw = protector.Unprotect(encrypted);
        return JsonSerializer.Deserialize<T>(raw, options ?? serializerOptions);
    }

    /// <inheritdoc/>
    public virtual T? UnProtect(string encrypted, JsonTypeInfo<T> jsonTypeInfo)
    {
        ArgumentNullException.ThrowIfNull(encrypted);

        var raw = protector.Unprotect(encrypted);
        return JsonSerializer.Deserialize(raw, jsonTypeInfo);
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public virtual T? UnProtect(string encrypted, out DateTimeOffset expiration, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(encrypted);

        var raw = timeLimitedProtector.Unprotect(encrypted, out expiration);
        return JsonSerializer.Deserialize<T>(raw, options ?? serializerOptions);
    }

    /// <inheritdoc/>
    public virtual T? UnProtect(string encrypted, out DateTimeOffset expiration, JsonTypeInfo<T> jsonTypeInfo)
    {
        ArgumentNullException.ThrowIfNull(encrypted);

        var raw = timeLimitedProtector.Unprotect(encrypted, out expiration);
        return JsonSerializer.Deserialize(raw, jsonTypeInfo);
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public virtual string Protect(T value, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        var raw = JsonSerializer.Serialize(value, options ?? serializerOptions);
        return protector.Protect(raw);
    }

    /// <inheritdoc/>
    public virtual string Protect(T value, JsonTypeInfo<T> jsonTypeInfo)
    {
        ArgumentNullException.ThrowIfNull(value);

        var raw = JsonSerializer.Serialize(value, jsonTypeInfo);
        return protector.Protect(raw);
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public virtual string Protect(T value, DateTimeOffset expiration, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        var raw = JsonSerializer.Serialize(value, options ?? serializerOptions);
        return timeLimitedProtector.Protect(raw, expiration);
    }

    /// <inheritdoc/>
    public virtual string Protect(T value, DateTimeOffset expiration, JsonTypeInfo<T> jsonTypeInfo)
    {
        ArgumentNullException.ThrowIfNull(value);

        var raw = JsonSerializer.Serialize(value, jsonTypeInfo);
        return timeLimitedProtector.Protect(raw, expiration);
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public virtual string Protect(T value, TimeSpan lifetime, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        var raw = JsonSerializer.Serialize(value, options ?? serializerOptions);
        return timeLimitedProtector.Protect(raw, lifetime);
    }

    /// <inheritdoc/>
    public virtual string Protect(T value, TimeSpan lifetime, JsonTypeInfo<T> jsonTypeInfo)
    {
        ArgumentNullException.ThrowIfNull(value);

        var raw = JsonSerializer.Serialize(value, jsonTypeInfo);
        return timeLimitedProtector.Protect(raw, lifetime);
    }
}
