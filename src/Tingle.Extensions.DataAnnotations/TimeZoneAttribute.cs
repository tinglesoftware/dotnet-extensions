using TimeZoneConverter;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value is a well-formed timezone identifier.
    /// This support both Windows and IANA timezone identifiers
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class TimeZoneAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneAttribute"/> class.
        /// </summary>
        public TimeZoneAttribute() : base("The field {0} must be a valid Windows or IANA TimeZone identifier.") { }

        /// <inheritdoc/>
        public override bool IsValid(object? value)
        {
            return value is not string s || string.IsNullOrEmpty(s) || TZConvert.TryGetTimeZoneInfo(windowsOrIanaTimeZoneId: s, out _);
        }
    }
}
