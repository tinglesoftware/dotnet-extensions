using Tingle.AspNetCore.JsonPatch.Exceptions;

namespace Tingle.AspNetCore.JsonPatch.Internal;

internal static class ErrorReporter
{
    public static readonly Action<JsonPatchError> Default = (error) =>
    {
        throw new JsonPatchException(error);
    };
}
