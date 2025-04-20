# Tingle.Extensions.Primitives

This library contains extra primitive types. These are additional to what the framework already provides. Some are inspired by community projects.

For some primitive types, there are also some validation attributes for easier validation of input data.

## Extra Primitive Types

### `ByteSize`

Handling of byte calculations. Extended from [`ByteSize`](https://github.com/omar/ByteSize).

```cs
var bs = ByteSize.FromKiloBytes(1.5);
Console.WriteLine(bs);
```

### `ConnectionStringBuilder`

Helps you with the manipulation of connection strings. In most SDKs provided by Microsoft the builders are usually internal and cannot be reused or are very usage centric. This type helps in the manipulation of connection strings either from the final string or parts and can then produce the final string or segments.

```cs
var cs = "HostName=contoso.com;Scheme=https;Key=abcd";
var csb = new ConnectionStringBuilder(cs);
Console.WriteLine("contoso.com", csb.GetHostname());
Console.WriteLine("https", csb.GetScheme());
Console.WriteLine("abcd", csb.GetKey());
```

### `Country`

Easier handling of countries with support for validation.

```cs
if (Country.TryGetFromCode('KEN', out var country))
{
    Console.WriteLine(country.Name); // Kenya
}
```

Supports validation via attributes too:

```cs
class TestModel1
{
    [CountryCode]
    public string? SomeCode { get; set; }
}
```

### `Currency`

Easier handling of currencies with support for validation.

```cs
if (Currency.TryGetFromCode('KES', out var currency))
{
    Console.WriteLine(currency.Name); // Kenyan Shilling
}
```

Supports validation via attributes too:

```cs
class TestModel1
{
    [Currency]
    public string? SomeCode { get; set; }
}
```

### `Duration`

`TimeSpan` is great but when dealing with values outside the .NET ecosystem you can either specify the time in seconds, milliseconds, or minutes.
This type provides an alternative, by using [ISO8601](https://en.wikipedia.org/wiki/ISO_8601) durations e.g. `PT10M` for 10 minutes.
It can be used to perform operations against `DateTime` and `DateTimeOffset` instances.

```cs
var ts = TimeSpan.FromDays(1.556); // 1 day, 13 hours, 20 minutes and 38 seconds
var duration = new Duration(ts);
Console.WriteLine(duration); // P1DT13H20M38S

DateTime dt = new(2022, 08, 31, 12, 19, 10);
var equal = dt.AddMonths(1) == dt + Duration.FromMonths(1);
```

### `Etag`

A convenience type for handling Etag values. For example, you can use this to read values from SQLServer's `RowVersion` or you can create your own version when working with MongoDB.
It is also useful when combining multiple values such when you want a new Etag if an entry in an array of items changes.

```cs
var etag = new Etag("0x75BCD15");
Console.WriteLine(etag.ToString("H")); // 0x75BCD15
Console.WriteLine(etag.ToString("B")); // Fc1bBwAAAAA=
Console.WriteLine(etag); // Fc1bBwAAAAA=
```

### `ImageDimensions`

Simple convenience for storing image dimensions similar to `System.Drawing.Size`.

```cs
var value = new ImageDimensions(50, 45);
Console.WriteLine(value); // 50px by 45px
```

### `ImageDimensionsRange`

A type that eases the check for whether an image's dimensions are contained in a given range.

```cs
var value = new ImageDimensionsRange(45, 50);
Console.WriteLine(value); // within (45px by 45px) and (50px by 50px)

var min = new ImageDimensions(45, 45);
var max = new ImageDimensions(100, 100);
var range = new ImageDimensionsRange(min, max);
var dimensions = new ImageDimensions(width, height);
Console.WriteLine(range.IsWithin(dimensions)); // true
```

### `Ksuid`

A K-Sortable Globally Unique Identifier, based on the reference implementation from [Segment](https://github.com/segmentio/ksuid).

```cs
var id = Ksuid.Generate(); // generation
Console.WriteLine(id);

if (Ksuid.TryParse("0o5Fs0EELR0fUjHjbCnEtdUwQe3", out var id)) {
    Console.WriteLine($"{id.Created:o}"); // 2017-05-17T01:49:21+00:00
}
```

### `Language`

Easier handling of languages with support for validation.

```cs
if (Language.TryGetFromCode('swa', out var language))
{
    Console.WriteLine(language.Name); // Swahili
}
```

Supports validation via attributes too:

```cs
class TestModel1
{
    [LanguageCode]
    public string? SomeCode { get; set; }
}
```

### `SequenceNumber`

A sequential number inspired by Twitter's (late) Snowflake project and Instagram's implementation.
This is very useful for situations where you want non-predictable identifiers.
It can also be used for sorting data in a database without exposing the field in APIs.

```cs
var sequenceNumber1 = SequenceNumber.Generate(); // generation
Console.WriteLine(sequenceNumber1);
```

### `SwiftCode`

Represents a SWIFT Code broken down into its components. The format of a Swift Code is as specified under ISO-9362.

```cs
var sw = SwiftCode.Parse("KCBLKENXXXX");
Console.WriteLine(sw.Institution); // "KCBL"
Console.WriteLine(sw.Country); // "KE"
Console.WriteLine(sw.Location); // "NX"
Console.WriteLine(sw.Branch); // "XXX"
```

### `Keygen`

Helper for generating non-sequential keys such as those use as client secrets, webhook secrets, signing keys, etc.
The keys can be generated from bytes, strings, and numbers where deterministic outputs are desired.

```cs
Console.WriteLine(Keygen.Create(BitConverter.GetBytes(100), Keygen.OutputFormat.Base64)); // ZAAAAAAAAAA=
Console.WriteLine(Keygen.Create(BitConverter.GetBytes(100), Keygen.OutputFormat.Hex)); // 6400000000000000
Console.WriteLine(Keygen.Create(BitConverter.GetBytes(100), Keygen.OutputFormat.Base62)); // 8aISBA7FdnE

Console.WriteLine(Keygen.Create("100", Keygen.OutputFormat.Base64, null)); // MTAw
Console.WriteLine(Keygen.Create("000", Keygen.OutputFormat.Hex, Encoding.ASCII)); // MDAw
Console.WriteLine(Keygen.Create("000", Keygen.OutputFormat.Hex, Encoding.Unicode)); // MAAwADAA
```

### Conversion

Most of the types have conversion to/from JSON using `System.Text.Json`.
Support for Type converters is also included to allow binding via `IConfiguration` instances.

| Type                      | JSON converter | Type converter |
| ------------------------- | -------------- | -------------- |
| `ByteSize`                | &#9745;        | &#9745;        |
| `ConnectionStringBuilder` | &#9745;        | &#9745;        |
| `Country`                 | &#9745;        | &#9745;        |
| `Currency`                | &#9745;        | &#9745;        |
| `Duration`                | &#9745;        | &#9745;        |
| `Etag`                    | &#9745;        | &#9745;        |
| `ImageDimensions`         | &#9744;        | &#9744;        |
| `ImageDimensionsRange`    | &#9744;        | &#9744;        |
| `Ksuid`                   | &#9745;        | &#9745;        |
| `Language`                | &#9745;        | &#9745;        |
| `SequenceNumber`          | &#9745;        | &#9745;        |
| `SwiftCode`               | &#9745;        | &#9745;        |
| `Keygen`                  | &#9744;        | &#9744;        |

There are also some convenience extension methods on framework types that can be useful for various tasks. They are explained below:

## Extensions for shortening numbers

You may want to convert the numeric values to its equivalent string representation. The output will be an abbreviated string. For example, you may want to represent `1,000,000` as `1M` or `1,000` as `1K`. This can be done as shown below:

```cs
long number = 1_000;
var converted = number.ToStringAbbreviated();
Console.WriteLine(converted); // 1K
```

You can use overloads for `ToStringAbbreviated(...)` to supply an `IFormatProvider` if you need to abbreviate a number using culture-specific formatting information.

## Extensions for string protection

Sometimes you may want to mask certain portions of strings that may contain sensitive information. An example is a password or an authentication key to access an API. An example of how you can do this is shown below:

```cs
var key = "EcsmGa/wXv/HlA==";
var result = key.Protect();
Console.WriteLine(result); // Ecs*************
```

By default, only the first 20% of the string will be kept as is. The remaining characters will be replaced with asterisks (\*). To alter this default behavior you can supply different values to the following parameters:

- `toKeep`: Specifies how many of the characters in the string aren't replaced with the replacement character (which is an asterisk by default) as a percentage.
- `position`: The position of the string to protect. By default, the `StringProtectionPosition` is `End`. You can alter this to `Middle` or `Start`.
- `replacementChar`: The character to replace the protected string characters with. By default, it is an asterisk (\*).
- `replacementLength`: The length of the replacement string using the replacement characters.

## Extensions for splitting a string in Pascal casing into multiple words

[Pascal case](https://www.pluralsight.com/blog/software-development/programming-naming-conventions-explained#pascal-case).

```cs
var str = "HomeAndAway";
var result = str.PascalSplit();
Console.WriteLine(result); // Home And Away
```

By default, the string will be split, and a single whitespace will be inserted in between the words. If you'd like to change the separator that is inserted between the words you can set the `separator` parameter in `PascalSplit(...)`.

## Extensions to the get the value declared on the Enum using EnumMemberAttribute

For example:

```cs
public enum Platform
{
    [EnumMember(Value = "ios")]
    iOS,

    Android
}
```

In the declared enumerations to represent the iOS and Android platforms. Note that only the iOS member has an `EnumMemberAttribute` annotated on it.

```cs
var platform = Platform.iOS;
var result = tt.GetEnumMemberAttrValue();
Console.WriteLine(result); // ios

platform = Platform.Android;
result = tt.GetEnumMemberAttrValue();
// result is null here
```

Since the `iOS` member is annotated with an `EnumMemberAttribute`, `ios` is returned as expected. However, the `Android` member doesn't have that annotation and thus the value that will be returned will be `null`. If you'd like to return the default value in lower case format when there are members without an `EnumMemberAttribute` annotation:

```cs
var platform = Platform.iOS;
var result = tt.GetEnumMemberAttrValueOrDefault();
Console.WriteLine(result); // ios

platform = Platform.Android;
result = tt.GetEnumMemberAttrValueOrDefault();
Console.WriteLine(result); // android
```
