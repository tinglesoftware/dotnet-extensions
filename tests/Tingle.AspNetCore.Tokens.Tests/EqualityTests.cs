using Microsoft.AspNetCore.Mvc;

namespace Tingle.AspNetCore.Tokens.Tests;

public class EqualityTests
{
    [Fact]
    public void ContinuationToken_Equals_ReturnsTrue()
    {
        var td1 = TestDataClass.CreateRandom();
        var td2 = TestDataClass.CreateRandom();

        var ct11 = new ContinuationToken<TestDataClass>(td1);
        var ct12 = new ContinuationToken<TestDataClass>(td1);

        var ct21 = new ContinuationToken<TestDataClass>(td2);
        var ct22 = new ContinuationToken<TestDataClass>(td2);

        Assert.Equal(ct11, ct12);
        Assert.Equal(ct21, ct22);
        Assert.NotEqual(ct11, ct21);
        Assert.NotEqual(ct11, ct22);
        Assert.NotEqual(ct11, ct22);
        Assert.NotEqual(ct12, ct22);

        Assert.True(ct11 == ct12);
        Assert.True(ct21 == ct22);
        Assert.False(ct11 == ct21);
        Assert.False(ct11 == ct22);
        Assert.False(ct11 == ct22);
        Assert.False(ct12 == ct22);

        // try with fixed ones
        ContinuationToken<TestDataClass>? ct31 = default;
        ContinuationToken<TestDataClass>? ct32 = null;
        Assert.NotNull(ct11);
        Assert.NotEqual(default, ct21);
        Assert.Null(ct31);
        Assert.Equal(default, ct32);
    }

    [Fact]
    public void TimedTimedContinuationToken_Equals_ReturnsTrue()
    {
        var td1 = TestDataClass.CreateRandom();
        var td2 = TestDataClass.CreateRandom();

        var exp1 = DateTimeOffset.UtcNow.AddDays(-1);
        var exp2 = DateTimeOffset.UtcNow.AddDays(-2);

        var tmspt11 = new TimedContinuationToken<TestDataClass>(td1, exp1);
        var tmspt12 = new TimedContinuationToken<TestDataClass>(td1, exp1);

        var tmspt21 = new TimedContinuationToken<TestDataClass>(td2, exp2);
        var tmspt22 = new TimedContinuationToken<TestDataClass>(td2, exp2);

        Assert.Equal(tmspt11, tmspt12);
        Assert.Equal(tmspt21, tmspt22);
        Assert.NotEqual(tmspt11, tmspt21);
        Assert.NotEqual(tmspt11, tmspt22);
        Assert.NotEqual(tmspt11, tmspt22);
        Assert.NotEqual(tmspt12, tmspt22);

        Assert.True(tmspt11 == tmspt12);
        Assert.True(tmspt21 == tmspt22);
        Assert.False(tmspt11 == tmspt21);
        Assert.False(tmspt11 == tmspt22);
        Assert.False(tmspt11 == tmspt22);
        Assert.False(tmspt12 == tmspt22);

        // try with fixed ones
        TimedContinuationToken<TestDataClass>? tmspt31 = default;
        TimedContinuationToken<TestDataClass>? tmspt32 = null;
        Assert.NotNull(tmspt11);
        Assert.NotEqual(default, tmspt21);
        Assert.Null(tmspt31);
        Assert.Equal(default, tmspt32);
    }
}
