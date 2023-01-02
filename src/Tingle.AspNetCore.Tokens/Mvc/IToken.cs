namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 
/// </summary>
public interface IToken
{
    /// <summary>
    /// Gets the original value.
    /// This is also the encrypted version of the raw value.
    /// </summary>
    public string? GetProtected();
}
