namespace Tingle.Extensions.Processing;

/// <summary>
/// A processor that processes a list of items sequentially based on a concurrency limit.
/// The processing order the items is guaranteed.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SequentialBatchProcessor<T>
{
    private readonly SemaphoreSlim concurrencyLimiter;
    private readonly Func<T, CancellationToken, Task> handler;

    /// <summary>Creates an instance of <see cref="SequentialBatchProcessor{T}"/>.</summary>
    /// <param name="concurrencyLimit">The maximum number of concurrent items</param>
    /// <param name="handler">The handler for item in the data. This handler shall be awaited.</param>
    public SequentialBatchProcessor(int concurrencyLimit = 1, Func<T, CancellationToken, Task>? handler = null)
    {
        concurrencyLimiter = new SemaphoreSlim(1, concurrencyLimit);
        this.handler = handler ?? ((s, c) => Task.CompletedTask);
    }


    /// <summary>Handle a single item.</summary>
    /// <param name="item">The item to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task HandleAsync(T item, CancellationToken cancellationToken = default)
    {
#pragma warning disable CAC001 // ConfigureAwaitChecker
        await handler(item, cancellationToken);
#pragma warning restore CAC001 // ConfigureAwaitChecker
    }

    /// <summary>Process items based on concurrency limit.</summary>
    /// <param name="items">The items to be processed.</param>
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
