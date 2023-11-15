namespace Tingle.AspNetCore.Authentication.SharedKey.Validation.Exceptions;

///
[Serializable]
public class SharedKeyTimeWindowExpiredException : Exception
{
    ///
    public SharedKeyTimeWindowExpiredException() { }

    ///
    public SharedKeyTimeWindowExpiredException(string message) : base(message) { }

    ///
    public SharedKeyTimeWindowExpiredException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// The date supplied
    /// </summary>
    public DateTimeOffset SuppliedTime { get; private set; }

    /// <summary>
    /// The date that was expected to not be older than
    /// </summary>
    public DateTimeOffset OldestAllowed { get; private set; }

    private const string MessageFormat = "The specified time {0} should not be after {1}";
    ///
    public static SharedKeyTimeWindowExpiredException Create(DateTimeOffset suppliedTime, DateTimeOffset oldestAllowed)
    {
        return new SharedKeyTimeWindowExpiredException(string.Format(MessageFormat, suppliedTime, oldestAllowed))
        {
            SuppliedTime = suppliedTime,
            OldestAllowed = oldestAllowed
        };
    }
}
