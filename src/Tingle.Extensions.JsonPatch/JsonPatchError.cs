using Tingle.Extensions.JsonPatch.Operations;

namespace Tingle.Extensions.JsonPatch;

/// <summary>
/// Captures error message and the related entity and the operation that caused it.
/// </summary>
/// <param name="affectedObject">The object that is affected by the error.</param>
/// <param name="operation">The <see cref="Operations.Operation"/> that caused the error.</param>
/// <param name="errorMessage">The error message.</param>
public class JsonPatchError(object affectedObject, Operation operation, string errorMessage)
{
    /// <summary>
    /// Gets the object that is affected by the error.
    /// </summary>
    public object AffectedObject { get; private set; } = affectedObject;

    /// <summary>
    /// Gets the <see cref="Operation"/> that caused the error.
    /// </summary>
    public Operation Operation { get; private set; } = operation;

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string ErrorMessage { get; private set; } = errorMessage;
}
