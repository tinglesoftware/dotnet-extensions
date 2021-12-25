using System;
using Tingle.Extensions.JsonPatch.Exceptions;

namespace Tingle.Extensions.JsonPatch.Internal;

internal static class ErrorReporter
{
    public static readonly Action<JsonPatchError> Default = (error) =>
    {
        throw new JsonPatchException(error);
    };
}
