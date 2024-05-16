namespace Tingle.AspNetCore.Swagger.Tests;

public class XmlCommentsHelperTests
{
    [Theory]
    [InlineData("<br />", "\r\n")]
    [InlineData("<br/>", "\r\n")]
    [InlineData("<br>", "\r\n")]
    public void ToMarkdown_Converts_Br(string input, string expected)
    {
        var actual = XmlCommentsHelper.ToMarkdown(input);
        Assert.Equal(expected, actual, false, true);
    }
}
