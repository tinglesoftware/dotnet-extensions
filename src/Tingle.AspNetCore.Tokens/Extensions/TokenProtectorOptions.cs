using Tingle.AspNetCore.Tokens.Protection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Options for configuring instances of <see cref="TokenProtector{T}"/>
/// </summary>
public class TokenProtectorOptions
{
    /// <summary>
    /// Indicates if the TokenProtector should use <see cref="object.ToString"/>
    /// or <see cref="IConvertible.ToString(System.IFormatProvider?)"/>
    /// instead of JSON serialization before protection and after unprotection.
    /// </summary>
    /// <remarks>
    /// It is better to use JSON for conversions of <see cref="DateTimeOffset"/>
    /// and <see cref="DateTime"/> to avoid loss in sub-second precision.
    /// </remarks>
    public bool UseConversionInsteadOfJson { get; set; }
}
