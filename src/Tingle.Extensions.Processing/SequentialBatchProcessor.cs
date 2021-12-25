namespace Tingle.Extensions.Processing;

/// <summary>
/// A processor that processes a list of items sequentially but can be parallelized without splitting.
/// The processing order the items is guaranteed.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SequentialBatchProcessor<T>
{
    private readonly SemaphoreSlim concurrencyLimiter;
    private readonly Func<T, CancellationToken, Task> handler;

    /// <summary>
    /// Creates a processor for bulk items
    /// </summary>
    /// <param name="concurrencyLimit">the maximum number of concurrent items</param>
    /// <param name="handler">the handler for item in the data. This handler shall be awaited.</param>
    public SequentialBatchProcessor(int concurrencyLimit = 1, Func<T, CancellationToken, Task>? handler = null)
    {
        concurrencyLimiter = new SemaphoreSlim(1, concurrencyLimit);
        this.handler = handler ?? ((s, c) => Task.CompletedTask);
    }


    /// <summary>
    /// Handles an items in the data
    /// </summary>
    /// <param name="item">the item to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task HandleAsync(T item, CancellationToken cancellationToken = default)
    {
#pragma warning disable CAC001 // ConfigureAwaitChecker
        await handler(item, cancellationToken);
#pragma warning restore CAC001 // ConfigureAwaitChecker
    }

    /// <summary>
    /// Split the items and process them in parallel
    /// </summary>
    /// <param name="items">the items to be processed</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ProcessAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        foreach (var item in items)
        {
            // ensure the operation has not been cancelled
            cancellationToken.ThrowIfCancellationRequested();

            // wait for the handle to be ready
            await concurrencyLimiter.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                // handle the item
                await HandleAsync(item, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                concurrencyLimiter.Release(); // release the handle to be used by someone else
            }
        }
    }
}
