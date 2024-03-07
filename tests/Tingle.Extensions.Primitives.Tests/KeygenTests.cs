using System.Text;

namespace Tingle.Extensions.Primitives.Tests;

public class KeygenTests
{
    [Theory]
    [InlineData(000, Keygen.OutputFormat.Base64, "AAAAAAAAAAA=")]
    [InlineData(100, Keygen.OutputFormat.Base64, "ZAAAAAAAAAA=")]
    [InlineData(000, Keygen.OutputFormat.Hex, "0000000000000000")]
    [InlineData(100, Keygen.OutputFormat.Hex, "6400000000000000")]
    [InlineData(000, Keygen.OutputFormat.Base62, "00000000")]
    [InlineData(100, Keygen.OutputFormat.Base62, "8aISBA7FdnE")]
    public void Create_Works(ulong value, Keygen.OutputFormat format, string expected)
    {
        Assert.Equal(expected, Keygen.Create(BitConverter.GetBytes(value), format));
    }

    [Theory]
    [MemberData(nameof(createTestData))]
    public void Create_Works_ForString(string key, Keygen.OutputFormat format, Encoding? encoding, string expected)
    {
        Assert.Equal(expected, Keygen.Create(key, format, encoding));
    }

    [Theory]
    [MemberData(nameof(decodeTestData))]
    public void Decode_Works(string key, Keygen.OutputFormat format, Encoding encoding, string expected)
    {
        Assert.Equal(expected, encoding.GetString(Keygen.Decode(key, format)));
    }

    public static readonly IEnumerable<object?[]> createTestData = new List<object?[]>
    {
        new object?[] { "000", Keygen.OutputFormat.Base64, null, "MDAw", },
        new object?[] { "100", Keygen.OutputFormat.Base64, null, "MTAw", },
        new object?[] { "000", Keygen.OutputFormat.Hex, null, "303030", },
        new object?[] { "100", Keygen.OutputFormat.Hex, null, "313030", },
        new object?[] { "000", Keygen.OutputFormat.Base62, null, "DFYW", },
        new object?[] { "100", Keygen.OutputFormat.Base62, null, "DWbY", },

        new object?[] { "000", Keygen.OutputFormat.Base64, Encoding.ASCII, "MDAw", },
        new object?[] { "000", Keygen.OutputFormat.Hex, Encoding.ASCII, "303030", },
        new object?[] { "000", Keygen.OutputFormat.Base62, Encoding.ASCII, "DFYW", },

        new object?[] { "000", Keygen.OutputFormat.Base64, Encoding.UTF32, "MAAAADAAAAAwAAAA", },
        new object?[] { "000", Keygen.OutputFormat.Hex, Encoding.UTF32, "300000003000000030000000", },
        new object?[] { "000", Keygen.OutputFormat.Base62, Encoding.UTF32, "JJpnu5vdqHZAdqG8", },

        new object?[] { "000", Keygen.OutputFormat.Base64, Encoding.Unicode, "MAAwADAA", },
        new object?[] { "000", Keygen.OutputFormat.Hex, Encoding.Unicode, "300030003000", },
        new object?[] { "000", Keygen.OutputFormat.Base62, Encoding.Unicode, "EzAr0VHM", },

        new object?[] {
            "idv_0o5Fs0EELR0fUjHjbCnEtdUwQe3:live:AAAAAAAAAAA=",
            Keygen.OutputFormat.Base62,
            null,
            "Cy0PU8LXdqtEkQlWzmln8wB5IRDPWHsUPcEUrloBAm20XJJOCheBOMp9ceYK9psrjZ",
        },

        new object?[] {
            "idv_0o5Fs0EELR0fUjHjbCnEtdUwQe3:AAAAAAAAAAA=",
            Keygen.OutputFormat.Base62,
            null,
            "fX8y3T8GYbqdnFDC38RpPvpd948XZsb5KkoeKCumHdDL05hVijwYhHaz0Pt",
        },

        new object?[] {
            "{\"id\":\"idv_0o5Fs0EELR0fUjHjbCnEtdUwQe3\",\"etag\":\"AAAAAAAAAAA=\"}",
            Keygen.OutputFormat.Base62,
            null,
            "1fy0BgWMw2g2RdIcbIBnv28reNqjJnEN38bkkpiWgWx8aew21SFbfScegTIJsw7S7ivONfnwKMlPaIgpib3V",
        },
    };

    public static readonly IEnumerable<object?[]> decodeTestData = new List<object?[]>
    {
        new object?[] { "MDAw", Keygen.OutputFormat.Base64, Encoding.UTF8, "000", },
        new object?[] { "MTAw", Keygen.OutputFormat.Base64, Encoding.UTF8, "100", },
        new object?[] { "303030", Keygen.OutputFormat.Hex, Encoding.UTF8, "000", },
        new object?[] { "313030", Keygen.OutputFormat.Hex, Encoding.UTF8, "100", },
        new object?[] { "DFYW", Keygen.OutputFormat.Base62, Encoding.UTF8, "000", },
        new object?[] { "DWbY", Keygen.OutputFormat.Base62, Encoding.UTF8, "100", },

        new object?[] { "MDAw", Keygen.OutputFormat.Base64, Encoding.ASCII, "000", },
        new object?[] { "300000003000000030000000", Keygen.OutputFormat.Hex, Encoding.UTF32, "000", },
        new object?[] { "EzAr0VHM", Keygen.OutputFormat.Base62, Encoding.Unicode, "000", },
    };
}
