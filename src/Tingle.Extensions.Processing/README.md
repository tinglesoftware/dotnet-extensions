# Tingle.Extensions.Processing

This library provides various processing functionalities that are commonly encountered by developers. 

The various functionalities will be described using examples below.

## Simplified Regex Matching

A simple one liner that searches the specified input string for the first occurrence that matches the regular expression.

```cs
// Create a pattern for a word that starts with letter "M"  
string pattern = @"\b[M]\w+";

// Create a Regex  
Regex rg = new Regex(pattern);

// Long string  
string authors = "Mahesh Chand, Raj Kumar, Mike Gold, Allen O'Neill, Marshal Troll";

if(rg.Match(authors, out Match? match))
{
    // your logic here...
}
```

## Set a Timeout For Tasks

You can set the amount of time to wait for a task. If the specified timeout is exceeded a `TimeoutException` will be thrown. This can be convenient especially when you have expensive operations being awaited.

Below is a sample usage:

```cs
var task = Task.Run(() => DoExpensiveOperationAsync(someParameter));
await task.WithTimeout(TimeSpan.FromSeconds(10));
```

## Attach a Fault Action To a Task

Sometimes execution of tasks can result in unhandled exceptions. This is normally indicated by the `Status` property transitioning to `TaskStatus.Faulted`. With this functionality, you can attach an `Action<Task>` or `Action<Task<TResult>>` that'll be executed if this happens.

Below is a sample usage:

```cs

Task HandleFaultAsync()
{
    //...Your logic here
    return Task.CompletedTask;
}

var task = Task.Run(() => DoExpensiveOperationThatMayThrowAsync(someParameter));
await task.OnFault(HandleFaultAsync);
```

An override of the `OnFault` method can allow the object state to be passed along for use by the continuation action.

## Reading from embedded resources

There are often times when we as developers need to read from files that have been included when compiling the assembly output.

In the .csproj file...

```cs
<ItemGroup>
  <EmbeddedResource Include="myfile.json" />
</ItemGroup>
```

Now, let us read the file

```csharp
internal class TestSamples
{
    var resource = EmbeddedResourceHelper.GetResourceAsStringAsync<TestSamples>("myfile.json");
}
```

Things to note in the function call above:

1. The `TestSamples` is a any class from the target resource's assembly. This is used to scope the manifest resource name.
2. `myfile.json` is the file we need to read from. This variable is case sensitive.
3. `resource` is the string content of the embedded resource. This can then be parsed or deserialized to your desired format such as JSON for example.

There is also support for reading embedded resources contained in a folder. Let us see how to handle this in an example:

In the .csproj file...

```cs
<ItemGroup>
  <EmbeddedResource Include="Samples\myfile.json" />
</ItemGroup>
```

```csharp
internal class TestSamples
{
    var resource = EmbeddedResourceHelper.GetResourceAsStringAsync<TestSamples>("Samples", "myfile.json");
}
```

Here we are also providing the `Samples` parameter as the folder name where the file we want to read from is contained. The folder name is also case sensitive similar to the embedded resource name.

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

Therefore, we can process the `tokens` in a squential manner. By default, `SequentialBatchProcessor<T>`'s constructor has the concurrency limit set to 1. If you wish to increase the number of items handled concurrently, you can increae this limit to a higher value.

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




