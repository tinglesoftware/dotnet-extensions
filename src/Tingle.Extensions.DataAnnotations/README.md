# Tingle.Extensions.DataAnnotations

Additional data validation attributes in the `System.ComponentModel.DataAnnotations` namespace. Some of this should have been present in the framework but are very specific to some use cases.

|AttributeName|Description|Remarks|
|--|--|--|
|`DateMustBeInTheFutureAttribute`|Specifies that a data field value is a date in the future.||
|`DateMustBeInThePastAttribute`|Specifies that a data field value is a date in the past.||
|`FiveStarRatingAttribute`|Specifies the decimal numeric range for the value of a data field must be between 0 and 5.||
|`GreaterThanZeroAttribute`|Specifies the integer numeric range for the value of a data field must be more than zero. Only works for integers.||
|`KRAPinAttribute`|Specifies that a data field value is a well-formed KRA PIN number using a regular expression for KRA Pins.<br/>The default expression to be matched is:<br/>`^[a-zA-Z][0-9]{9}[a-zA-Z]$`||
|`PrefixAttribute`|Specifies that a data field value starts with a specified string.||
|`SuffixAttribute`|Specifies that a data field value ends with a specified string.||
|`SwiftCodeAttribute`|Specifies that a data field value is a well-formed SWIFT Code using a regular expression for SWIFT Codes as specified under ISO-9362.<br/>The expression to be matched is <br/>`^([a-zA-Z]{4})([a-zA-Z]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{3})?$`||
|`TenStarRatingAttribute`|Specifies the decimal numeric range for the value of a data field must be between 0 and 10.||
