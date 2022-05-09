namespace Tingle.Extensions.Processing.Tests;

public class SequentialBatchProcessorTests
{
    [Fact]
    public async Task ProcessAsync_Works_Concurrency_1()
    {
        var numbers = Enumerable.Range(1, 100);
        var invocations = 0;
        var processed = new List<int>();
        var processor = new SequentialBatchProcessor<int>(5, (n, ct) =>
        {
            Interlocked.Increment(ref invocations);
            processed.Add(n);
            return Task.CompletedTask;
        });
        await processor.ProcessAsync(numbers);
        Assert.Equal(100, invocations);
        Assert.Equal(numbers, processed);
    }

    [Fact]
    public async Task ProcessAsync_Works_Concurrency_3()
    {
        var numbers = Enumerable.Range(1, 100);
        var invocations = 0;
        var processed = new System.Collections.Concurrent.ConcurrentBag<int>();
        var processor = new SequentialBatchProcessor<int>(3, async (n, ct) =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(new Random(Guid.NewGuid().GetHashCode()).Next(1, 10)), ct);
            Interlocked.Increment(ref invocations);
            processed.Add(n);
        });
        await processor.ProcessAsync(numbers);
        Assert.Equal(100, invocations);
        Assert.Equal(numbers, processed.OrderBy(x => x));
    }
}
