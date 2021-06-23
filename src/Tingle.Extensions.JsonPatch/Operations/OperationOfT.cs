﻿using System;
using Tingle.Extensions.JsonPatch.Adapters;
using Tingle.Extensions.JsonPatch.Exceptions;
using Tingle.Extensions.JsonPatch.Properties;

namespace Tingle.Extensions.JsonPatch.Operations
{
    public class Operation<TModel> : Operation where TModel : class
    {
        public Operation() { }

        public Operation(string op, string path, string? from, object? value) : base(op, path, from)
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            if (path == null) throw new ArgumentNullException(nameof(path));

            this.value = value;
        }

        public Operation(string op, string path, string? from) : base(op, path, from)
        {
            if (op == null) throw new ArgumentNullException(nameof(op));
            if (path == null) throw new ArgumentNullException(nameof(path));
        }

        public void Apply(TModel objectToApplyTo, IObjectAdapter adapter)
        {
            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (adapter == null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            switch (OperationType)
            {
                case OperationType.Add:
                    adapter.Add(this, objectToApplyTo);
                    break;
                case OperationType.Remove:
                    adapter.Remove(this, objectToApplyTo);
                    break;
                case OperationType.Replace:
                    adapter.Replace(this, objectToApplyTo);
                    break;
                case OperationType.Move:
                    adapter.Move(this, objectToApplyTo);
                    break;
                case OperationType.Copy:
                    adapter.Copy(this, objectToApplyTo);
                    break;
                case OperationType.Test:
                    if (adapter is IObjectAdapterWithTest adapterWithTest)
                    {
                        adapterWithTest.Test(this, objectToApplyTo);
                        break;
                    }
                    else
                    {
                        throw new JsonPatchException(new JsonPatchError(objectToApplyTo, this, Resources.TestOperationNotSupported));
                    }
                case OperationType.Invalid:
                    throw new JsonPatchException(
                        Resources.FormatInvalidJsonPatchOperation(op), innerException: null);
                default:
                    break;
            }
        }
    }
}
