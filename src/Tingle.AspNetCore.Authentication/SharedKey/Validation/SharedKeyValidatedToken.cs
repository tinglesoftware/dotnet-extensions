namespace Tingle.AspNetCore.Authentication.SharedKey.Validation;

/// <summary>
/// The validation response produced after authentication
/// </summary>
public class SharedKeyValidatedToken(string token, string matchingKey)
{
    /// <summary>
    /// The token parsed
    /// </summary>
    public string Token { get; internal set; } = token ?? throw new ArgumentNullException(nameof(token));

    /// <summary>
    /// The key that matched the signature specified by <see cref="Token"/>
    /// </summary>
    public string MatchingKey { get; internal set; } = matchingKey ?? throw new ArgumentNullException(nameof(matchingKey));
}
