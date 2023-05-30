namespace Tingle.AspNetCore.Authentication.SharedKey.Validation;

/// <summary>
/// The validation response produced after authentication
/// </summary>
public class SharedKeyValidatedToken
{
    ///
    public SharedKeyValidatedToken(string token, string matchingKey)
    {
        Token = token ?? throw new ArgumentNullException(nameof(token));
        MatchingKey = matchingKey ?? throw new ArgumentNullException(nameof(matchingKey));
    }

    /// <summary>
    /// The token parsed
    /// </summary>
    public string Token { get; internal set; }

    /// <summary>
    /// The key that matched the signature specified by <see cref="Token"/>
    /// </summary>
    public string MatchingKey { get; internal set; }
}
