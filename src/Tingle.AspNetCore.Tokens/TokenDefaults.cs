namespace Tingle.AspNetCore.Tokens;

/// <summary>
/// Defaults for tokens
/// </summary>
public static class TokenDefaults
{
    /// <summary>
    /// The default header name to use in the response that includes a valid continuation token
    /// </summary>
    public const string ContinuationTokenHeaderName = "X-Continuation-Token";

    // change this name when the encrypting process changes
    internal const string ProtectorPurpose = "Tingle.AspNetCore.Tokens.v2024-05-05";
}
