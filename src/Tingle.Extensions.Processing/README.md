# Tingle.Extensions.Processing

This library provides various processing functionalities that are commonly encountered by developers.

The various functionalities will be described using examples below.

## Sequential Batch Processing

There are scenarios when we need to do some sort of processing of items in an `IEnumerable` but wish to do so in a parallel fashion without splitting up the items so as to ensure processing order of items is guaranteed.

Let us look at an example of how we can help to accomplish this.

First let us define a function that will be used to handle the processing of a variable of type `string`. The type to be processed can be any .NET type.

```csharp
Task DoStuffAsync(string token, CancellationToken ct)
{
    // Do your processing here...
    return Task.CompletedTask;
}
```

Let us now define a list of strings that require to be processed

```csharp
var tokens = new List<string> { "value_1", "value_2", "value_3"};
```

Now let us tie it all up together to show how we can process `tokens` sequentially.

```csharp
var processor = new SequentialBatchProcessor<string>(concurrencyLimit: 1, handler: (t, ct) => DoStuffAsync(token: t, ct: ct));
await processor.ProcessAsync(tokens, cancellationToken);
```

Therefore, we can process the `tokens` in a sequential manner. By default, `SequentialBatchProcessor<T>`'s constructor has the concurrency limit set to 1. If you wish to increase the number of items handled concurrently, you can increase this limit to a higher value.

## Split and Batch Processing

With this functionality, you can split a list of items and then process the splits in parallel. For example, a list of 2,000 items with a batchSize of 100 to produce 20 batches which would be processed in parallel.

However, unlike the Sequential Batch Processing described above, the processing order isn't guaranteed.

Using the previously defined `tokens` variable let us see how we can accomplish this:

First define a function that can process a list of items.

```csharp
Task DoBulkStuffAsync(List<string> tokens, CancellationToken ct)
{
    // Do your processing here...
    return Task.CompletedTask;
}
```

```csharp
var processor = new SplitAndBatchProcessor<string>(batchSize: 10, handler: (slice, ct) => DoBulkStuffAsync(slice, ct));
await processor.ProcessAsync(items: tokens, cancellationToken);
```

The list of `tokens` will be split into batches of 10 and then the slices will be processed in parallel. The batch size is set to 10 by default but a developer can change this to suit their own specific requirements.
