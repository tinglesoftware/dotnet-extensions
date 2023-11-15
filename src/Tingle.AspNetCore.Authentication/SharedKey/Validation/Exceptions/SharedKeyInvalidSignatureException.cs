namespace Tingle.AspNetCore.Authentication.SharedKey.Validation.Exceptions;

///
[Serializable]
public class SharedKeyInvalidSignatureException : Exception
{
    ///
    public SharedKeyInvalidSignatureException() { }

    ///
    public SharedKeyInvalidSignatureException(string message) : base(message) { }

    ///
    public SharedKeyInvalidSignatureException(string message, Exception inner) : base(message, inner) { }
}
