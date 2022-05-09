namespace Tingle.Extensions.Processing;

/// <summary>
/// A processor that splits a list of items and processes sections (splits) in parallel.
/// For example, a list of 2,000 items with a batchSize of 100 to produce 20 bacthes which would be processed in parallel.
/// The processing order the items is not guaranteed.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SplitAndBatchProcessor<T>
{
    private readonly int batchSize;
    private readonly Func<IEnumerable<T>, CancellationToken, Task> handler;

    /// <summary>Creates an instance of <see cref="SplitAndBatchProcessor{T}"/>.</summary>
    /// <param name="batchSize">The maximum number of items in a batch</param>
    /// <param name="handler">The handler for each batch of data. This handler is not be awaited, so as to ensure parallelism</param>
    public SplitAndBatchProcessor(int batchSize = 10, Func<IEnumerable<T>, CancellationToken, Task>? handler = null)
    {
        this.batchSize = batchSize;
        this.handler = handler ?? ((s, c) => Task.CompletedTask);
    }

    /// <summary>Handle a batch of items.</summary>
    /// <param name="batch">The batch of items to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task HandleAsync(IEnumerable<T> batch, CancellationToken cancellationToken = default)
    {
#pragma warning disable CAC001 // ConfigureAwaitChecker
        await handler(batch, cancellationToken);
#pragma warning restore CAC001 // ConfigureAwaitChecker
    }

    /// <summary>Split the items and process them in parallel.</summary>
    /// <param name="items">The items to be processed.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ProcessAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        // we group them into batches then we parallelize the batches
        // example: if we have 50 batches and a batchSize of 100 items, we'll have process 5,000
        //          messages in total but only 50 at a time in parallel
#if NET6_0_OR_GREATER
        var batches = items.Chunk(batchSize).ToList();
#else
        var list = items.ToList();
        var count = list.Count / batchSize + (list.Count % batchSize > 0 ? 1 : 0);
        var batches = new List<IReadOnlyCollection<T>>();
        for (var i = 0; i < count; i++)
        {
            var batch = list.GetRange(i * batchSize, Math.Min(batchSize, list.Count - i * batchSize));
            batches.Add(batch);
        }
#endif

        // wait for all tasks in parallel
        var tasks = batches.Select(b => HandleAsync(b, cancellationToken));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
