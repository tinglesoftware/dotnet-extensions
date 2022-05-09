namespace Tingle.Extensions.Processing.Tests;

public class SplitAndBatchProcessorTests
{
    [Fact]
    public async Task ProcessAsync_Works()
    {
        var numbers = Enumerable.Range(1, 100);
        var invocations = 0;
        var processed = new System.Collections.Concurrent.ConcurrentBag<int>();
        var processor = new SplitAndBatchProcessor<int>(5, async (b, ct) =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(new Random(Guid.NewGuid().GetHashCode()).Next(1, 400)), ct);
            Interlocked.Increment(ref invocations);
            foreach (var n in b) processed.Add(n);
        });
        await processor.ProcessAsync(numbers);
        Assert.Equal(20, invocations);
        Assert.Equal(100, processed.Count);
        Assert.Equal(numbers, processed.OrderBy(x => x));
    }
}
