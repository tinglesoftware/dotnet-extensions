using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
    private readonly Func<List<T>, CancellationToken, Task> handler;

    /// <summary>
    /// Creates a processor for bulk items
    /// </summary>
    /// <param name="batchSize">the maximum number of items in a batch</param>
    /// <param name="handler">the handler for each batch of data. This handler is not be awaited, so as to ensure parallelism</param>
    public SplitAndBatchProcessor(int batchSize = 10, Func<List<T>, CancellationToken, Task>? handler = null)
    {
        this.batchSize = batchSize;
        this.handler = handler ?? ((s, c) => Task.CompletedTask);
    }

    /// <summary>
    /// Handles a batch of data
    /// </summary>
    /// <param name="batch">the portion of items to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task HandleAsync(List<T> batch, CancellationToken cancellationToken = default)
    {
#pragma warning disable CAC001 // ConfigureAwaitChecker
        await handler(batch, cancellationToken);
#pragma warning restore CAC001 // ConfigureAwaitChecker
    }

    /// <summary>
    /// Split the items and process them in parallel
    /// </summary>
    /// <param name="items">the items to be batched</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ProcessAsync(List<T> items, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>();

        // we group them into batches then we parallelize the batches
        // example: if we have 50 batches and a batchSize of 100 items, we'll have process 5,000
        //          messages in total but only 50 at a time in parallel
        var batchesCount = items.Count / batchSize + (items.Count % batchSize > 0 ? 1 : 0);
        for (var i = 0; i < batchesCount; i++)
        {
            var batch = items.GetRange(i * batchSize, Math.Min(batchSize, items.Count - i * batchSize));
            tasks.Add(HandleAsync(batch, cancellationToken));
        }

        // wait for all tasks in parallel
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
