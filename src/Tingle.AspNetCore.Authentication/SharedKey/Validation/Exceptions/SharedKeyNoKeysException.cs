namespace Tingle.AspNetCore.Authentication.SharedKey.Validation.Exceptions;

///
[Serializable]
public class SharedKeyNoKeysException : Exception
{
    ///
    public SharedKeyNoKeysException() { }

    ///
    public SharedKeyNoKeysException(string message) : base(message) { }

    ///
    public SharedKeyNoKeysException(string message, Exception inner) : base(message, inner) { }
}
