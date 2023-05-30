using System.Runtime.Serialization;

namespace Tingle.AspNetCore.Authentication.SharedKey.Validation.Exceptions;

///
[Serializable]
public class SharedKeyInvalidDateException : Exception
{
    ///
    public SharedKeyInvalidDateException() { }

    ///
    public SharedKeyInvalidDateException(string message) : base(message) { }

    ///
    public SharedKeyInvalidDateException(string message, Exception inner) : base(message, inner) { }

    ///
    protected SharedKeyInvalidDateException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    /// <summary>
    /// The value supplied for date
    /// </summary>
    public string? Value { get; private set; }

    private const string MessageFormat = "The specified time header value ({0}) is invalid";
    ///
    public static SharedKeyInvalidDateException Create(string value)
    {
        return new SharedKeyInvalidDateException(string.Format(MessageFormat, value)) { Value = value };
    }
}
