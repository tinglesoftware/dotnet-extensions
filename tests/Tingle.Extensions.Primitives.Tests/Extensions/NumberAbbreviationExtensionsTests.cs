namespace Tingle.Extensions.Primitives.Tests.Extensions;

public class NumberAbbreviationExtensionsTests
{
    [Theory]
    [InlineData(1_001, "1K")]
    [InlineData(1_010, "1K")]
    [InlineData(10_300, "10.3K")]
    [InlineData(3_900_120, "3.9M")]
    [InlineData(3_910_120, "3.91M")]
    [InlineData(3_000_120, "3M")]
    [InlineData(1_400_000_120, "1.4B")]
    [InlineData(1_000_000_120, "1B")]
    [InlineData(1_004_000_120, "1.004B")]
    [InlineData(1_044_000_120, "1.044B")]
    public void ToStringAbbreviated_Int(int original, string excepected)
    {
        var actual = original.ToStringAbbreviated();
        Assert.Equal(excepected, actual);
    }

    [Theory]
    [InlineData(1_001, "1K")]
    [InlineData(1_010, "1K")]
    [InlineData(10_300, "10.3K")]
    [InlineData(3_900_120, "3.9M")]
    [InlineData(3_910_120, "3.91M")]
    [InlineData(3_000_120, "3M")]
    [InlineData(1_400_000_120, "1.4B")]
    [InlineData(1_000_000_120, "1B")]
    [InlineData(1_004_000_120, "1.004B")]
    [InlineData(1_044_000_120, "1.044B")]
    [InlineData(10_044_000_120, "10.044B")]
    public void ToStringAbbreviated_Long(long original, string excepected)
    {
        var actual = original.ToStringAbbreviated();
        Assert.Equal(excepected, actual);
    }
}
