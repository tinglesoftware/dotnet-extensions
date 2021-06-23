using PhoneNumbers;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value is a well-formed MSISDN phone number.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class MsisdnPhoneAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsisdnPhoneAttribute"/> class.
        /// </summary>
        public MsisdnPhoneAttribute() : base("The field {0} must be a valid MSISDN phone number.") { }

        /// <inheritdoc/>
        public override bool IsValid(object? value)
        {
            if (value is not string s || string.IsNullOrEmpty(s)) return true;
            try
            {
                var prepended = "+" + s;
                var phoneNumberUtil = PhoneNumberUtil.GetInstance();
                _ = phoneNumberUtil.Parse(numberToParse: prepended, defaultRegion: null);
                return true;
            }
            catch (Exception ex) when (ex is NumberParseException)
            {
                return false;
            }
        }
    }
}
