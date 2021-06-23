using System;
using Tingle.Extensions.JsonPatch.Operations;

namespace Tingle.Extensions.JsonPatch.Exceptions
{
    public class JsonPatchException : Exception
    {
        public Operation? FailedOperation { get; private set; }
        public object? AffectedObject { get; private set; }


        public JsonPatchException() { }

        public JsonPatchException(JsonPatchError jsonPatchError, Exception? innerException)
            : base(jsonPatchError.ErrorMessage, innerException)
        {
            FailedOperation = jsonPatchError.Operation;
            AffectedObject = jsonPatchError.AffectedObject;
        }

        public JsonPatchException(JsonPatchError jsonPatchError) : this(jsonPatchError, null) { }

        public JsonPatchException(string message, Exception? innerException) : base(message, innerException) { }
    }
}
