namespace Tingle.AspNetCore.Authentication.SharedKey;

/// <summary>
/// Defaults for <see cref="SharedKeyOptions"/>
/// </summary>
public static class SharedKeyDefaults
{
    /// <summary>
    /// Default authentication scheme for <see cref="SharedKeyOptions"/>
    /// </summary>
    public const string AuthenticationScheme = "SharedKey";

    /// <summary>
    /// Default value for <see cref="SharedKeyOptions.HeaderValuePrefix"/>
    /// </summary>
    public const string HeaderPrefix = "SharedKey";

    /// <summary>
    /// Default value for <see cref="Validation.SharedKeyTokenValidationParameters.DateHeaderNames"/>
    /// </summary>
    public const string DateHeaderName = "x-ts-date";
}
