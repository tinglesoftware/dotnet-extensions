using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Tests;

[JsonSerializable(typeof(ByteSizeTests.TestModel), TypeInfoPropertyName = "ByteSizeTests_TestModel")]
[JsonSerializable(typeof(ConnectionStringBuilderTests.TestModel), TypeInfoPropertyName = "ConnectionStringBuilderTests_TestModel")]
[JsonSerializable(typeof(ContinentTests.TestModel), TypeInfoPropertyName = "ContinentTests_TestModel")]
[JsonSerializable(typeof(CountryTests.TestModel), TypeInfoPropertyName = "CountryTests_TestModel")]
[JsonSerializable(typeof(CurrencyTests.TestModel), TypeInfoPropertyName = "CurrencyTests_TestModel")]
[JsonSerializable(typeof(DurationTests.TestModel), TypeInfoPropertyName = "DurationTests_TestModel")]
[JsonSerializable(typeof(EtagTests.TestModel), TypeInfoPropertyName = "EtagTests_TestModel")]
[JsonSerializable(typeof(KsuidTests.TestModel), TypeInfoPropertyName = "KsuidTests_TestModel")]
[JsonSerializable(typeof(LanguageTests.TestModel), TypeInfoPropertyName = "LanguageTests_TestModel")]
[JsonSerializable(typeof(MoneyTests.TestModel), TypeInfoPropertyName = "MoneyTests_TestModel")]
[JsonSerializable(typeof(SequenceNumberTests.TestModel), TypeInfoPropertyName = "SequenceNumberTests_TestModel")]
[JsonSerializable(typeof(SwiftCodeTests.TestModel), TypeInfoPropertyName = "SwiftCodeTests_TestModel")]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class TestJsonSerializerContext : JsonSerializerContext { }
