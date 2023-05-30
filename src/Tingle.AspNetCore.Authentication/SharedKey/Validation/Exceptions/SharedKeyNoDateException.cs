using System.Runtime.Serialization;

namespace Tingle.AspNetCore.Authentication.SharedKey.Validation.Exceptions;

///
[Serializable]
public class SharedKeyNoDateException : Exception
{
    ///
    public SharedKeyNoDateException() { }

    ///
    public SharedKeyNoDateException(string message) : base(message) { }

    ///
    public SharedKeyNoDateException(string message, Exception inner) : base(message, inner) { }

    ///
    protected SharedKeyNoDateException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    /// <summary>
    /// List of possible header names for specifying the time
    /// </summary>
    public IEnumerable<string> PossibleHeaderNames { get; private set; } = new List<string>();

    internal string? HeaderNamesJoined { get; private set; }

    private const string MessageFormat = "The date header must be specified with any of these names ({0})";
    ///
    public static SharedKeyNoDateException Create(IEnumerable<string> possibleHeaderNames)
    {
        var headerNamesJoined = string.Join(", ", possibleHeaderNames);
        return new SharedKeyNoDateException(string.Format(MessageFormat, headerNamesJoined))
        {
            PossibleHeaderNames = possibleHeaderNames,
            HeaderNamesJoined = headerNamesJoined,
        };
    }
}
