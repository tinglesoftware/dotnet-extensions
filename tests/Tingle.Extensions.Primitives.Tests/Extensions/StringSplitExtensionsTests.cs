namespace Tingle.Extensions.Primitives.Tests.Extensions;

public class StringSplitExtensionsTests
{
    [Theory]
    [InlineData("HomeAndAway", " ", "Home And Away")]
    [InlineData("HomeAndAway", "_", "Home_And_Away")]
    [InlineData("Chickensoup", " ", "Chickensoup")]
    public void PascalSplit_works(string original, string separator, string expected)
    {
        var actual = original.PascalSplit(separator);
        Assert.Equal(expected, actual);
    }
}
