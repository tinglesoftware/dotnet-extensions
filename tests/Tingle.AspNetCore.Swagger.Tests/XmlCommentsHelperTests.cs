namespace Tingle.AspNetCore.Swagger.Tests;

public class XmlCommentsHelperTests
{
    [Theory]
    [InlineData(@"Check <see href=""https://wikipedia.org"">Wikipedia</see> for more.", "Check [Wikipedia](https://wikipedia.org) for more.")]
    [InlineData(@"<see href=""https://www.iso.org/iso-4217-currency-codes.html"">ISO currency code</see>", "[ISO currency code](https://www.iso.org/iso-4217-currency-codes.html)")]
    public void ToMarkdown_Converts_SeeHref(string input, string expected)
    {
        var actual = XmlCommentsHelper.ToMarkdown(input);
        Assert.Equal(expected, actual, false, true);
    }

    [Theory]
    [InlineData("<br />", "\r\n")]
    [InlineData("<br/>", "\r\n")]
    [InlineData("<br>", "\r\n")]
    public void ToMarkdown_Converts_Br(string input, string expected)
    {
        var actual = XmlCommentsHelper.ToMarkdown(input);
        Assert.Equal(expected, actual, false, true);
    }

    [Fact]
    public void ToMarkdown_Converts_All()
    {
        var input = @"Check <see href=""https://wikipedia.org"">Wikipedia</see> for more."
                 + "<br />";
        var expected = "Check [Wikipedia](https://wikipedia.org) for more."
                     + "\r\n";
        var actual = XmlCommentsHelper.ToMarkdown(input);
        Assert.Equal(expected, actual, false, true);
    }
}
