using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives.Tests.Converters;

public class JsonStringEnumMemberConverterTests
{
    [Fact]
    public void AsDictionaryKeyTest()
    {
        var dict = new Dictionary<EnumDefinition, string>
        {
            [EnumDefinition.First] = "Mercedes",
            [EnumDefinition.Second] = "Toyota",
        };

        var options = new JsonSerializerOptions();
        var json = JsonSerializer.Serialize(dict, options);
        Assert.Equal(@"{""First"":""Mercedes"",""Second"":""Toyota""}", json);

        options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumMemberConverter(allowIntegerValues: true));
        json = JsonSerializer.Serialize(dict, options);
        Assert.Equal(@"{""First"":""Mercedes"",""_second"":""Toyota""}", json);

        var dict_rev = JsonSerializer.Deserialize<Dictionary<EnumDefinition, string>>(json, options)!;
        Assert.Equal<KeyValuePair<EnumDefinition, string>>(dict, dict_rev);
    }

    [Fact]
    public void EnumMemberSerializationTest()
    {
        string Json = JsonSerializer.Serialize(FlagDefinitions.Four);
        Assert.Equal(@"""four value""", Json);

        Json = JsonSerializer.Serialize(FlagDefinitions.Four | FlagDefinitions.One);
        Assert.Equal(@"""one value, four value""", Json);

        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumMemberConverter(allowIntegerValues: true));
        Json = JsonSerializer.Serialize((FlagDefinitions)255, options);
        Assert.Equal("255", Json);
    }

    [Fact]
    public void EnumMemberDeserializationTest()
    {
        FlagDefinitions Value = JsonSerializer.Deserialize<FlagDefinitions>(@"""all values""");
        Assert.Equal(FlagDefinitions.All, Value);

        Value = JsonSerializer.Deserialize<FlagDefinitions>(@"""two value, three value""");
        Assert.Equal(FlagDefinitions.Two | FlagDefinitions.Three, Value);

        Value = JsonSerializer.Deserialize<FlagDefinitions>(@"""tWo VALUE""");
        Assert.Equal(FlagDefinitions.Two, Value);
    }

    [Theory]
    [InlineData("null")]
    [InlineData(@"""invalid_value""")]
    public void EnumMemberInvalidDeserializationTest(string json) => Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<FlagDefinitions>(json));

    [Fact]
    public void EnumMemberInvalidNumericValueDeserializationTest()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumMemberConverter(allowIntegerValues: false));

        Assert.Throws<JsonException>(() => JsonSerializer.Serialize((FlagDefinitions)255, options));
    }

    [Fact]
    public void NullableEnumSerializationTest()
    {
        var Options = new JsonSerializerOptions();
        Options.Converters.Add(new JsonStringEnumMemberConverter(allowIntegerValues: true));

        string Json = JsonSerializer.Serialize((DayOfWeek?)null, Options);
        Assert.Equal("null", Json);

        Json = JsonSerializer.Serialize((DayOfWeek?)DayOfWeek.Monday, Options);
        Assert.Equal(@"""Monday""", Json);

        Json = JsonSerializer.Serialize((EnumDefinition?)255, Options);
        Assert.Equal("255", Json);
    }

    [Fact]
    public void NullableEnumDeserializationTest()
    {
        var Options = new JsonSerializerOptions();
        Options.Converters.Add(new JsonStringEnumMemberConverter(allowIntegerValues: true));

        DayOfWeek? Value = JsonSerializer.Deserialize<DayOfWeek?>("null", Options);
        Assert.Null(Value);

        Value = JsonSerializer.Deserialize<DayOfWeek?>(@"""Friday""", Options);
        Assert.Equal(DayOfWeek.Friday, Value);

        EnumDefinition? EnumValue = JsonSerializer.Deserialize<EnumDefinition?>(@"""fIrSt""", Options);
        Assert.Equal(EnumDefinition.First, EnumValue);

        EnumValue = JsonSerializer.Deserialize<EnumDefinition?>(@"255", Options);
        Assert.Equal(255, (int)EnumValue!);
    }

    [Fact]
    public void EnumMemberSerializationOptionsTest()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumMemberConverter(JsonNamingPolicy.CamelCase) }
        };

        string json = JsonSerializer.Serialize(EnumDefinition.First, options);
        Assert.Equal(@"""first""", json);

        json = JsonSerializer.Serialize(EnumDefinition.Second, options);
        Assert.Equal(@"""_second""", json);
    }

    [Fact]
    public void EnumMemberDeserializationOptionsTest()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumMemberConverter(JsonNamingPolicy.CamelCase) }
        };

        EnumDefinition Value = JsonSerializer.Deserialize<EnumDefinition>(@"""first""", options);
        Assert.Equal(EnumDefinition.First, Value);

        Value = JsonSerializer.Deserialize<EnumDefinition>(@"""_second""", options);
        Assert.Equal(EnumDefinition.Second, Value);
    }

    [Fact]
    public void EnumMemberInvalidDeserializationOptionsTest()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumMemberConverter() }
        };

        var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<EnumDefinition>(@"""invalid_value""", options));
    }

    [Fact]
    public void EnumMemberInvalidTypeDeserializationOptionsTest()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumMemberConverter(allowIntegerValues: false) }
        };

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<EnumDefinition>(@"255", options));
    }

    [Fact]
    public void EnumMemberInvalidDeserializationIncludesJsonPathInMessageTest()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumMemberConverter() }
        };

        var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<EnumDefinition>(@"""invalid_value""", options));
        Assert.Contains(". Path: $", ex.Message);
    }

    [Fact]
    public void EnumMemberFlagInvalidDeserializationIncludesJsonPathInMessageTest()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumMemberConverter() }
        };

        var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<FlagDefinitions>(@"""invalid_value""", options));
        Assert.Contains(". Path: $", ex.Message);
    }

    [Fact]
    public void JsonPropertyNameSerializationTest()
    {
        string Json = JsonSerializer.Serialize(MixedEnumDefinition.First);
        Assert.Equal(@"""_first""", Json);

        Json = JsonSerializer.Serialize(MixedEnumDefinition.Second);
        Assert.Equal(@"""_second""", Json);

        Json = JsonSerializer.Serialize(MixedEnumDefinition.Third);
        Assert.Equal(@"""_third_enumMember""", Json);
    }

    [Fact]
    public void JsonPropertyNameDeserializationTest()
    {
        MixedEnumDefinition Value = JsonSerializer.Deserialize<MixedEnumDefinition>(@"""_first""");
        Assert.Equal(MixedEnumDefinition.First, Value);

        Value = JsonSerializer.Deserialize<MixedEnumDefinition>(@"""_second""");
        Assert.Equal(MixedEnumDefinition.Second, Value);

        Value = JsonSerializer.Deserialize<MixedEnumDefinition>(@"""_third_enumMember""");
        Assert.Equal(MixedEnumDefinition.Third, Value);
    }

    [Fact]
    public void WorksWithGenericConverter()
    {
        var dict = new Dictionary<EnumWithGenericConverter, string>
        {
            [EnumWithGenericConverter.First] = "Mercedes",
            [EnumWithGenericConverter.Second] = "Toyota",
        };

        var options = new JsonSerializerOptions();
        var json = JsonSerializer.Serialize(dict, options);
        Assert.Equal(@"{""First"":""Mercedes"",""_second"":""Toyota""}", json);

        var dict_rev = JsonSerializer.Deserialize<Dictionary<EnumWithGenericConverter, string>>(json, options)!;
        Assert.Equal<KeyValuePair<EnumWithGenericConverter, string>>(dict, dict_rev);
    }

    [Flags]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum FlagDefinitions
    {
        //None = 0x00,

        [EnumMember(Value = "all values")]
        All = One | Two | Three | Four,

        [EnumMember(Value = "one value")]
        One = 0x01,

        [EnumMember(Value = "two value")]
        Two = 0x02,

        [EnumMember(Value = "three value")]
        [JsonPropertyName("jsonPropertyName.is.ignored")]
        Three = 0x04,

        [JsonPropertyName("four value")]
        Four = 0x08,
    }

    public enum EnumDefinition
    {
        First,

        [EnumMember(Value = "_second")]
        Second,
    }

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum MixedEnumDefinition
    {
        [EnumMember(Value = "_first")]
        First,

        [JsonPropertyName("_second")]
        Second,

        // Note: We use EnumMember over JsonPropertyName if both are specified.
        [JsonPropertyName("_third_jsonPropertyName")]
        [EnumMember(Value = "_third_enumMember")]
        Third
    }

    [JsonConverter(typeof(JsonStringEnumMemberConverter<EnumWithGenericConverter>))]
    public enum EnumWithGenericConverter
    {
        First,

        [EnumMember(Value = "_second")]
        Second,
    }
}
