using System.Runtime.Serialization;

namespace Tingle.AspNetCore.Authentication.SharedKey.Validation.Exceptions;

///
[Serializable]
public class SharedKeyTokenException : Exception
{
    ///
    public SharedKeyTokenException() { }

    ///
    public SharedKeyTokenException(string message) : base(message) { }

    ///
    public SharedKeyTokenException(string message, Exception inner) : base(message, inner) { }

    ///
    protected SharedKeyTokenException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
