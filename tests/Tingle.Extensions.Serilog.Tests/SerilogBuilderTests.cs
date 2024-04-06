using Serilog;

namespace Tingle.Extensions.Serilog.Tests;

public class SerilogBuilderTests
{
    [Fact]
    public void Merge_Works()
    {
        var actions = new List<Action<LoggerConfiguration>>();
        var merged = SerilogBuilder.Merge(actions);
        Assert.Null(merged);

        actions.Add(config => { });
        merged = SerilogBuilder.Merge(actions);
        Assert.NotNull(merged);
    }
}
