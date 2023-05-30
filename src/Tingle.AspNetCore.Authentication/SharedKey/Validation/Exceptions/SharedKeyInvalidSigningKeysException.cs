using System.Runtime.Serialization;

namespace Tingle.AspNetCore.Authentication.SharedKey.Validation.Exceptions;

///
[Serializable]
public class SharedKeyInvalidSigningKeysException : Exception
{
    ///
    public SharedKeyInvalidSigningKeysException() { }

    ///
    public SharedKeyInvalidSigningKeysException(string message) : base(message) { }

    ///
    public SharedKeyInvalidSigningKeysException(string message, Exception inner) : base(message, inner) { }

    ///
    protected SharedKeyInvalidSigningKeysException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    /// <summary>
    /// List of invalid keys
    /// </summary>
    public IEnumerable<string>? Keys { get; private set; }

    private const string MessageFormat = "Invalid keys specified ({0}).";
    ///
    public static SharedKeyInvalidSigningKeysException Create(List<string> keys)
    {
        var keysJoined = string.Join(", ", keys);
        return new SharedKeyInvalidSigningKeysException(string.Format(MessageFormat, keysJoined)) { Keys = keys };
    }
}
