namespace Tingle.AspNetCore.Tokens.Tests;

class TestDataClass
{
    public string? S1 { get; set; }
    public bool B1 { get; set; }
    public int I1 { get; set; }
    public double D1 { get; set; }

    public override string ToString() => string.Join(",", S1, B1, I1, D1);

    public override bool Equals(object? obj) => string.Equals(ToString(), obj?.ToString());

    public override int GetHashCode() => ToString().GetHashCode();

    public static TestDataClass CreateRandom()
    {
        var rnd = new Random();

        return new TestDataClass
        {
            B1 = true,
            D1 = rnd.NextDouble(),
            I1 = rnd.Next(),
            S1 = Guid.NewGuid().ToString()
        };
    }
}
