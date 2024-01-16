namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is a well-formed KRA Pin number using a regular expression for KRA Pins.
/// The default expression to be matched is <c>^[a-zA-Z][0-9]{9}[a-zA-Z]$</c>
/// </summary>
/// <param name="pattern">
/// The regular expression that is used to validate the data field value.
/// Defaults to <c>^[a-zA-Z][0-9]{9}[a-zA-Z]$</c>
/// </param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class KRAPinAttribute(string pattern = "^[a-zA-Z][0-9]{9}[a-zA-Z]$") : RegularExpressionAttribute(pattern) { }
