using System.Text.Json;
using Tingle.AspNetCore.JsonPatch.Internal;
using Tingle.AspNetCore.JsonPatch.Operations;
using Tingle.AspNetCore.JsonPatch.Properties;

namespace Tingle.AspNetCore.JsonPatch.Adapters;

/// <inheritdoc />
/// <param name="serializerOptions">The <see cref="JsonSerializerOptions"/>.</param>
/// <param name="logErrorAction">The <see cref="Action"/> for logging <see cref="JsonPatchError"/>.</param>
/// <param name="adapterFactory">The <see cref="IAdapterFactory"/> to use when creating adaptors.</param>
public class ObjectAdapter(JsonSerializerOptions serializerOptions, Action<JsonPatchError>? logErrorAction, IAdapterFactory adapterFactory, bool create) : IObjectAdapterWithTest
{
    /// <summary>
    /// Initializes a new instance of <see cref="ObjectAdapter"/>.
    /// </summary>
    /// <param name="serializerOptions">The <see cref="JsonSerializerOptions"/>.</param>
    /// <param name="logErrorAction">The <see cref="Action"/> for logging <see cref="JsonPatchError"/>.</param>
    public ObjectAdapter(
        JsonSerializerOptions serializerOptions,
        Action<JsonPatchError>? logErrorAction,
        bool create) :
        this(serializerOptions, logErrorAction, Adapters.AdapterFactory.Default, create)
    {
    }

    /// <summary>
    /// Gets or sets the <see cref="JsonSerializerOptions"/>.
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; } = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));

    /// <summary>
    /// Gets or sets the <see cref="IAdapterFactory"/>
    /// </summary>
    public IAdapterFactory AdapterFactory { get; } = adapterFactory ?? throw new ArgumentNullException(nameof(adapterFactory));

    /// <summary>
    /// Action for logging <see cref="JsonPatchError"/>.
    /// </summary>
    public Action<JsonPatchError>? LogErrorAction { get; } = logErrorAction;

    public void Add(Operation operation, object objectToApplyTo)
    {
        ArgumentNullException.ThrowIfNull(operation);

        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        Add(operation.path, operation.value, objectToApplyTo, operation);
    }

    /// <summary>
    /// Add is used by various operations (eg: add, copy, ...), yet through different operations;
    /// This method allows code reuse yet reporting the correct operation on error
    /// </summary>
    private void Add(
        string path,
        object? value,
        object objectToApplyTo,
        Operation operation)
    {
        ArgumentNullException.ThrowIfNull(path);

        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        ArgumentNullException.ThrowIfNull(operation);

        var parsedPath = new ParsedPath(path);
        var visitor = new ObjectVisitor(parsedPath, SerializerOptions, AdapterFactory, create);

        var target = objectToApplyTo;
        if (!visitor.TryVisit(ref target, out var adapter, out var errorMessage))
        {
            var error = CreatePathNotFoundError(objectToApplyTo, path, operation, errorMessage);
            ErrorReporter(error);
            return;
        }

        if (!adapter.TryAdd(target, parsedPath.LastSegment!, SerializerOptions, value, out errorMessage))
        {
            var error = CreateOperationFailedError(objectToApplyTo, path, operation, errorMessage);
            ErrorReporter(error);
            return;
        }
    }

    public void Move(Operation operation, object objectToApplyTo)
    {
        ArgumentNullException.ThrowIfNull(operation);

        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        // Get value at 'from' location and add that value to the 'path' location
        if (TryGetValue(operation.from!, objectToApplyTo, operation, out var propertyValue))
        {
            // remove that value
            Remove(operation.from!, objectToApplyTo, operation);

            // add that value to the path location
            Add(operation.path,
                propertyValue,
                objectToApplyTo,
                operation);
        }
    }

    public void Remove(Operation operation, object objectToApplyTo)
    {
        ArgumentNullException.ThrowIfNull(operation);

        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        Remove(operation.path, objectToApplyTo, operation);
    }

    /// <summary>
    /// Remove is used by various operations (eg: remove, move, ...), yet through different operations;
    /// This method allows code reuse yet reporting the correct operation on error.  The return value
    /// contains the type of the item that has been removed (and a bool possibly signifying an error)
    /// This can be used by other methods, like replace, to ensure that we can pass in the correctly
    /// typed value to whatever method follows.
    /// </summary>
    private void Remove(string path, object objectToApplyTo, Operation operationToReport)
    {
        var parsedPath = new ParsedPath(path);
        var visitor = new ObjectVisitor(parsedPath, SerializerOptions, AdapterFactory, create);

        var target = objectToApplyTo;
        if (!visitor.TryVisit(ref target, out var adapter, out var errorMessage))
        {
            var error = CreatePathNotFoundError(objectToApplyTo, path, operationToReport, errorMessage);
            ErrorReporter(error);
            return;
        }

        if (!adapter.TryRemove(target, parsedPath.LastSegment!, SerializerOptions, out errorMessage))
        {
            var error = CreateOperationFailedError(objectToApplyTo, path, operationToReport, errorMessage);
            ErrorReporter(error);
            return;
        }
    }

    public void Replace(Operation operation, object objectToApplyTo)
    {
        ArgumentNullException.ThrowIfNull(operation);

        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        var parsedPath = new ParsedPath(operation.path);
        var visitor = new ObjectVisitor(parsedPath, SerializerOptions, AdapterFactory, create);

        var target = objectToApplyTo;
        if (!visitor.TryVisit(ref target, out var adapter, out var errorMessage))
        {
            var error = CreatePathNotFoundError(objectToApplyTo, operation.path, operation, errorMessage);
            ErrorReporter(error);
            return;
        }

        if (!adapter.TryReplace(target, parsedPath.LastSegment!, SerializerOptions, operation.value, out errorMessage))
        {
            var error = CreateOperationFailedError(objectToApplyTo, operation.path, operation, errorMessage);
            ErrorReporter(error);
            return;
        }
    }

    public void Copy(Operation operation, object objectToApplyTo)
    {
        ArgumentNullException.ThrowIfNull(operation);

        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        // Get value at 'from' location and add that value to the 'path' location
        if (TryGetValue(operation.from!, objectToApplyTo, operation, out var propertyValue))
        {
            // Create deep copy
            var copyResult = ConversionResultProvider.CopyTo(propertyValue, propertyValue?.GetType()!);
            if (copyResult.CanBeConverted)
            {
                Add(operation.path,
                    copyResult.ConvertedInstance,
                    objectToApplyTo,
                    operation);
            }
            else
            {
                var error = CreateOperationFailedError(objectToApplyTo, operation.path, operation, Resources.FormatCannotCopyProperty(operation.from));
                ErrorReporter(error);
                return;
            }
        }
    }

    public void Test(Operation operation, object objectToApplyTo)
    {
        ArgumentNullException.ThrowIfNull(operation);

        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        var parsedPath = new ParsedPath(operation.path);
        var visitor = new ObjectVisitor(parsedPath, SerializerOptions, AdapterFactory, create);

        var target = objectToApplyTo;
        if (!visitor.TryVisit(ref target, out var adapter, out var errorMessage))
        {
            var error = CreatePathNotFoundError(objectToApplyTo, operation.path, operation, errorMessage);
            ErrorReporter(error);
            return;
        }

        if (!adapter.TryTest(target, parsedPath.LastSegment!, SerializerOptions, operation.value, out errorMessage))
        {
            var error = CreateOperationFailedError(objectToApplyTo, operation.path, operation, errorMessage);
            ErrorReporter(error);
            return;
        }
    }

    private bool TryGetValue(
        string fromLocation,
        object objectToGetValueFrom,
        Operation operation,
        out object? propertyValue)
    {
        ArgumentNullException.ThrowIfNull(fromLocation);

        ArgumentNullException.ThrowIfNull(objectToGetValueFrom);

        ArgumentNullException.ThrowIfNull(operation);

        propertyValue = null;

        var parsedPath = new ParsedPath(fromLocation);
        var visitor = new ObjectVisitor(parsedPath, SerializerOptions, AdapterFactory, create);

        var target = objectToGetValueFrom;
        if (!visitor.TryVisit(ref target, out var adapter, out var errorMessage))
        {
            var error = CreatePathNotFoundError(objectToGetValueFrom, fromLocation, operation, errorMessage);
            ErrorReporter(error);
            return false;
        }

        if (!adapter.TryGet(target, parsedPath.LastSegment!, SerializerOptions, out propertyValue, out errorMessage))
        {
            var error = CreateOperationFailedError(objectToGetValueFrom, fromLocation, operation, errorMessage);
            ErrorReporter(error);
            return false;
        }

        return true;
    }

    private Action<JsonPatchError> ErrorReporter
    {
        get
        {
            return LogErrorAction ?? Internal.ErrorReporter.Default;
        }
    }

    private static JsonPatchError CreateOperationFailedError(object target, string path, Operation operation, string? errorMessage)
    {
        return new JsonPatchError(
            target,
            operation,
            errorMessage ?? Resources.FormatCannotPerformOperation(operation.op, path));
    }

    private static JsonPatchError CreatePathNotFoundError(object target, string path, Operation operation, string? errorMessage)
    {
        return new JsonPatchError(
            target,
            operation,
            errorMessage ?? Resources.FormatTargetLocationNotFound(operation.op, path));
    }
}
