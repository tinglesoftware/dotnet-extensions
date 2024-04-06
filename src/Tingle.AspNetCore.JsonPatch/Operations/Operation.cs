using System.Text.Json.Serialization;
using Tingle.AspNetCore.JsonPatch.Adapters;
using Tingle.AspNetCore.JsonPatch.Properties;

namespace Tingle.AspNetCore.JsonPatch.Operations;

#pragma warning disable IDE1006 // Naming Styles

public class Operation
{
    private string _op = default!;
    private OperationType _operationType;

    [JsonIgnore]
    public OperationType OperationType => _operationType;

    [JsonPropertyName("path")]
    public string path { get; set; } = default!;

    [JsonPropertyName("op")]
    public string op
    {
        get => _op;
        set
        {
            if (!Enum.TryParse(value, ignoreCase: true, result: out OperationType result))
            {
                result = OperationType.Invalid;
            }
            _operationType = result;
            _op = value;
        }
    }

    [JsonPropertyName("from")]
    public string? from { get; set; }

    [JsonPropertyName("value")]
    public object? value { get; set; }

    public Operation() { }

    public Operation(string op, string path, string? from)
    {
        this.op = op ?? throw new ArgumentNullException(nameof(op));
        this.path = path ?? throw new ArgumentNullException(nameof(path));
        this.from = from;
    }

    public Operation(string op, string path, string? from, object? value) : this(op, path, from)
    {
        this.value = value;
    }

    public void Apply(object objectToApplyTo, IObjectAdapter adapter)
    {
        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        ArgumentNullException.ThrowIfNull(adapter);

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
                    throw new NotSupportedException(Resources.TestOperationNotSupported);
                }
            default:
                break;
        }
    }
}
