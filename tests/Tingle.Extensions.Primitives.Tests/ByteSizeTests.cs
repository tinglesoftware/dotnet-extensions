using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives.Tests;

public class ByteSizeTests
{
    #region Creation

    [Fact]
    public void Constructor_Works()
    {
        long bytes = 1125899906842624;
        var result = new ByteSize(bytes);
        Assert.Equal(bytes, result.Bytes);
        Assert.Equal(bytes / 1024, result.KibiBytes);
        Assert.Equal(bytes / 1024 / 1024, result.MebiBytes);
        Assert.Equal(bytes / 1024 / 1024 / 1024, result.GibiBytes);
        Assert.Equal(bytes / 1024 / 1024 / 1024 / 1024, result.TebiBytes);
        Assert.Equal(1, result.PebiBytes);
    }

    [Fact]
    public void FromBytes_Works()
    {
        var result = ByteSize.FromKiloBytes(1.5);
        Assert.Equal(1500, result.Bytes);
        Assert.Equal(1.5, result.KiloBytes);

        result = ByteSize.FromMegaBytes(1.5);
        Assert.Equal(1500000, result.Bytes);
        Assert.Equal(1.5, result.MegaBytes);

        result = ByteSize.FromGigaBytes(1.5);
        Assert.Equal(1500000000, result.Bytes);
        Assert.Equal(1.5, result.GigaBytes);

        result = ByteSize.FromTeraBytes(1.5);
        Assert.Equal(1500000000000, result.Bytes);
        Assert.Equal(1.5, result.TeraBytes);

        result = ByteSize.FromPetaBytes(1.5);
        Assert.Equal(1500000000000000, result.Bytes);
        Assert.Equal(1.5, result.PetaBytes);

        result = ByteSize.FromKibiBytes(1.5);
        Assert.Equal(1536, result.Bytes);
        Assert.Equal(1.5, result.KibiBytes);

        result = ByteSize.FromMebiBytes(1.5);
        Assert.Equal(1572864, result.Bytes);
        Assert.Equal(1.5, result.MebiBytes);

        result = ByteSize.FromGibiBytes(1.5);
        Assert.Equal(1610612736, result.Bytes);
        Assert.Equal(1.5, result.GibiBytes);

        result = ByteSize.FromTebiBytes(1.5);
        Assert.Equal(1649267441664, result.Bytes);
        Assert.Equal(1.5, result.TebiBytes);

        result = ByteSize.FromPebiBytes(1.5);
        Assert.Equal(1688849860263936, result.Bytes);
        Assert.Equal(1.5, result.PebiBytes);
    }

    #endregion

    [Fact]
    public void Add_Works()
    {
        var size1 = ByteSize.FromBytes(1);
        var result = size1.Add(size1);
        Assert.Equal(2, result.Bytes);
    }

    [Fact]
    public void Subtract_Works()
    {
        var size = ByteSize.FromBytes(4).Subtract(ByteSize.FromBytes(2));
        Assert.Equal(2, size.Bytes);
    }

    [Fact]
    public void MinusOperatorUnary_Works()
    {
        var size = ByteSize.FromBytes(2);
        size = -size;
        Assert.Equal(-2, size.Bytes);
    }

    [Fact]
    public void MinusOperatorBinary_Works()
    {
        var size = ByteSize.FromBytes(4) - ByteSize.FromBytes(2);
        Assert.Equal(2, size.Bytes);
    }

    [Fact]
    public void PlusOperatorBinary_Works()
    {
        var size = ByteSize.FromBytes(4) + ByteSize.FromBytes(2);
        Assert.Equal(6, size.Bytes);
    }

    [Fact]
    public void MultiplyOperator_Works()
    {
        var a = ByteSize.FromBytes(2);
        var b = ByteSize.FromBytes(2);
        var actual = a * b;
        Assert.Equal(4, actual.Bytes);
    }

    [Fact]
    public void DivideOperator_Works()
    {
        var a = ByteSize.FromBytes(4);
        var b = ByteSize.FromBytes(2);
        var actual = a / b;
        Assert.Equal(2, actual.Bytes);
    }

    #region ToString

    [Fact]
    public void ToString_Works_VariousSizes()
    {
        Assert.Equal("0 B", ByteSize.FromBytes(0).ToString());

        Assert.Equal("10 KB", ByteSize.FromKiloBytes(10).ToString("##.#### KB"));
        Assert.Equal("10 MB", ByteSize.FromMegaBytes(10).ToString("##.#### MB"));
        Assert.Equal("10 GB", ByteSize.FromGigaBytes(10).ToString("##.#### GB"));
        Assert.Equal("10 TB", ByteSize.FromTeraBytes(10).ToString("##.#### TB"));
        Assert.Equal("10 PB", ByteSize.FromPetaBytes(10).ToString("##.#### PB"));

        Assert.Equal("10 KiB", ByteSize.FromKibiBytes(10).ToString("##.#### KiB"));
        Assert.Equal("10 MiB", ByteSize.FromMebiBytes(10).ToString("##.#### MiB"));
        Assert.Equal("10 GiB", ByteSize.FromGibiBytes(10).ToString("##.#### GiB"));
        Assert.Equal("10 TiB", ByteSize.FromTebiBytes(10).ToString("##.#### TiB"));
        Assert.Equal("10 PiB", ByteSize.FromPebiBytes(10).ToString("##.#### PiB"));
    }

    [Fact]
    public void ToString_Returns_SelectedFormat()
    {
        var b = ByteSize.FromTeraBytes(10);
        var result = b.ToString("0.0 TB");
        Assert.Equal(10.ToString("0.0 TB"), result);

        b = ByteSize.FromTebiBytes(10);
        result = b.ToString("0.0 TiB");
        Assert.Equal(10.ToString("0.0 TiB"), result);
    }

    [Fact]
    public void ToString_Returns_DefaultRepresentation()
    {
        var b = ByteSize.FromKiloBytes(10);
        var result = b.ToBinaryString(CultureInfo.InvariantCulture);
        Assert.Equal("9.77 KiB", result);

        b = ByteSize.FromKiloBytes(10);
        var s = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        result = b.ToBinaryString(CultureInfo.CurrentCulture);
        Assert.Equal($"9{s}77 KiB", result);
    }

    [Fact]
    public void ToString_Returns_LargestSuffix()
    {
        var b = ByteSize.FromKiloBytes(10.5);
        var result = b.ToString();
        Assert.Equal(10.5.ToString("0.0 KB"), result);

        b = ByteSize.FromMegaBytes(.5);
        result = b.ToString("#.#");
        Assert.Equal("500 KB", result);

        b = ByteSize.FromMegaBytes(-.5);
        result = b.ToString("#.#");
        Assert.Equal("-500 KB", result);

        b = ByteSize.FromKiloBytes(9800);
        result = b.ToString("#.#", new CultureInfo("fr-FR"));
        Assert.Equal("9,8 MB", result);

        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
        b = ByteSize.FromKiloBytes(9770);
        result = b.ToString();
        Assert.Equal("9,77 MB", result);
        CultureInfo.CurrentCulture = originalCulture;
    }

    [Fact]
    public void ToString_Returns_CorrectNumberFormat()
    {
        var b = ByteSize.FromKiloBytes(10.5);
        var result = b.ToString("KB");
        Assert.Equal(10.5.ToString("0.0 KB"), result);

        b = ByteSize.FromMegaBytes(10.1234);
        result = b.ToString("#.#### MB");
        Assert.Equal(10.1234.ToString("0.0000 MB"), result);
    }

    [Fact]
    public void ToString_Returns_CultureSpecificFormat()
    {
        var b = ByteSize.FromKiloBytes(10.5);
        var deCulture = new CultureInfo("de-DE");
        var result = b.ToString("0.0 KB", deCulture);
        Assert.Equal(10.5.ToString("0.0 KB", deCulture), result);
    }

    #endregion

    #region TryParse

    [Fact]
    public void TryParse_Works()
    {
        var expected = ByteSize.FromKibiBytes(1020);
        var resultBool = ByteSize.TryParse("1020KiB", out var size);
        Assert.True(resultBool);
        Assert.Equal(expected, size);
    }

    [Fact]
    public void TryParse_Returns_False_BadValue()
    {
        var resultBool = ByteSize.TryParse("Unexpected Value", out var size);
        Assert.False(resultBool);
        Assert.Equal(new ByteSize(), size);
    }

    [Fact]
    public void TryParse_Returns_False_MissingSymbol()
    {
        var resultBool = ByteSize.TryParse("1000", out var size);
        Assert.False(resultBool);
        Assert.Equal(new ByteSize(), size);
    }

    [Fact]
    public void TryParse_Returns_False_MissingValue()
    {
        var resultBool = ByteSize.TryParse("KiB", out var size);
        Assert.False(resultBool);
        Assert.Equal(new ByteSize(), size);
    }

    [Fact]
    public void TryParse_Works_ExtraSpaces()
    {
        var expected = ByteSize.FromKibiBytes(100);
        var result = ByteSize.Parse(" 100 KiB ");
        Assert.Equal(expected, result);
    }

    #endregion

    #region Parse

    [Fact]
    public void Parse_Throws_FormatException_PartialBytes()
    {
        Assert.Throws<FormatException>(() => ByteSize.Parse("10.5B", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Parse_Throws_FormatException_BadValue()
    {
        Assert.Throws<FormatException>(() => ByteSize.Parse("Unexpected Value"));
    }

    [Fact]
    public void Parse_Throws_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ByteSize.Parse(null!));
    }


    [Fact]
    public void Parse_Works_CultureNumberSeparatorBinary()
    {
        var current = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");
        var expected = ByteSize.FromMebiBytes(1500.5);
        var result = ByteSize.Parse("1.500,5 MiB");
        Assert.Equal(expected, result);
        CultureInfo.CurrentCulture = current;
    }

    [Fact]
    public void Parse_Works_SuppliedCulture()
    {
        var cultureInfo = new CultureInfo("de-DE");
        var expected = ByteSize.FromMegaBytes(1234.5);
        var result = ByteSize.Parse("1.234,5 MB", cultureInfo);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_Works_CultureNumberSeparatorDecimal()
    {
        var current = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");
        var expected = ByteSize.FromMegaBytes(1500.5);
        var result = ByteSize.Parse("1.500,5 MB");
        Assert.Equal(expected, result);
        CultureInfo.CurrentCulture = current;
    }

    [Theory]
    [InlineData("1B", 1)]
    [InlineData("1020KiB", 1_020 * ByteSize.BytesInKibiByte)]
    [InlineData("1000MiB", 1_000 * ByteSize.BytesInMebiByte)]
    [InlineData("805GiB", 805 * ByteSize.BytesInGibiByte)]
    [InlineData("100TiB", 100 * ByteSize.BytesInTebiByte)]
    [InlineData("100PiB", 100 * ByteSize.BytesInPebiByte)]
    [InlineData("1020KB", 1_020 * ByteSize.BytesInKiloByte)]
    [InlineData("1000MB", 1_000 * ByteSize.BytesInMegaByte)]
    [InlineData("805GB", 805 * ByteSize.BytesInGigaByte)]
    [InlineData("100TB", 100 * ByteSize.BytesInTeraByte)]
    [InlineData("100PB", 100 * ByteSize.BytesInPetaByte)]
    [InlineData("100.5MB", (long)(100.5 * ByteSize.BytesInMegaByte))]
    public void Parse_Works(string val, long value)
    {
        var expected = new ByteSize(value);
        var result = ByteSize.Parse(val);
        Assert.Equal(expected, result);
    }

    #endregion

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = ByteSize.FromKiloBytes(1);
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(ByteSize), null)); // not AreSame because of boxing
        Assert.Throws<InvalidCastException>(() => Convert.ToBoolean(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToByte(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToChar(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToDateTime(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToDecimal(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToDouble(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToInt64(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToSByte(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToSingle(value));
        Assert.Equal("1 KB", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt64(value));

        Assert.Equal("1 KB", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Throws<InvalidCastException>(() => ((IConvertible)value).ToType(typeof(ulong), null));
    }

    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(ByteSize));
        Assert.NotNull(converter);
        var bs = ByteSize.FromBytes(1024);
        var actual = converter.ConvertToString(bs);
        var expected = "1 KiB";
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("1 KiB", ByteSize.BytesInKibiByte)]
    [InlineData("1 MiB", ByteSize.BytesInMebiByte)]
    [InlineData("1 GiB", ByteSize.BytesInGibiByte)]
    public void TypeConverter_ConvertsFromString(string input, long bytes)
    {
        var expected = new ByteSize(bytes: bytes);
        var converter = TypeDescriptor.GetConverter(typeof(ByteSize));
        Assert.NotNull(converter);
        var actual = Assert.IsType<ByteSize>(converter.ConvertFromString(input));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("1 KiB", ByteSize.BytesInKibiByte)]
    [InlineData("1 GiB", ByteSize.BytesInGibiByte)]
    public void JsonConverter_Works(string raw, long val)
    {
        var src_json = $"{{\"size\":\"{raw}\"}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(src_json, dst_json);
        Assert.Equal(val, model!.Size.Bytes);
    }

    [Theory]
    [InlineData(ByteSize.BytesInMebiByte, true, "1 MiB")]
    [InlineData(ByteSize.BytesInMebiByte, false, "1.05 MB")]
    public void JsonConverter_RespectsBinaryFormat(long val, bool binary, string expected)
    {
        var src_json = $"{{\"size\":\"{new ByteSize(bytes: val)}\"}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        options.Converters.Add(new ByteSizeJsonConverter(binary));
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var expected_json = $"{{\"size\":\"{expected}\"}}";
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(expected_json, dst_json);
    }

    class TestModel
    {
        public ByteSize Size { get; set; }
    }
}
