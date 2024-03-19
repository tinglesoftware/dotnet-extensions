namespace Tingle.Extensions.Primitives.Tests.Extensions;

public class StringProtectionExtensionsTests
{
    [Theory]
    [InlineData("EcsmGa/wXv/HlA==", "Ecs*************")]
    [InlineData("U6G0be/Q5wR1nExscY6Rfg==", "U6G0b*******************")]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90***********************************")]
    public void Protect_Works(string original, string expected)
    {
        Assert.Equal(expected, original.Protect());
    }

    [Theory]
    [InlineData("EcsmGa/wXv/HlA==", "Ecs*************", 0.2f)]
    [InlineData("U6G0be/Q5wR1nExscY6Rfg==", "U6G0b*******************", 0.2f)]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90***********************************", 0.2f)]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90CfdKbtcWg**************************", 0.4f)]
    public void Protect_Works_Respects_Fraction(string original, string expected, float fraction)
    {
        Assert.Equal(expected, original.Protect(toKeep: fraction));
    }

    [Theory]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90***********************************", StringProtectionPosition.End)]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "***********************************ttJAsQVU=", StringProtectionPosition.Start)]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gN************************************QVU=", StringProtectionPosition.Middle)]
    public void Protect_Respects_Position(string original, string expected, StringProtectionPosition position)
    {
        Assert.Equal(expected, original.Protect(position: position));
    }

    [Theory]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90***********************************", '*')]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", 'x')]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90-----------------------------------", '-')]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$", '$')]
    public void Protect_Respects_ReplacementChar(string original, string expected, char replacementChar)
    {
        Assert.Equal(expected, original.Protect(replacementChar: replacementChar));
    }

    [Theory]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90***********************************", null)]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90***********************************", 44)]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90*****", 5)]
    [InlineData("e0gNHBa90CfdKbtcWgksn52cvXoXMqCTaLdttJAsQVU=", "e0gNHBa90***********************************", 0)]
    public void Protect_Respects_ReplacementLength(string original, string expected, int? replacementLength)
    {
        Assert.Equal(expected, original.Protect(replacementLength: replacementLength));
    }
}
