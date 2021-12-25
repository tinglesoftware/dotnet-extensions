using System.Text.Json.Serialization;

namespace Tingle.Extensions.JsonPatch.Operations;

public class OperationBase
{
    private string? _op;
    private OperationType _operationType;

    [JsonIgnore]
    public OperationType OperationType
    {
        get
        {
            return _operationType;
        }
    }

    [JsonPropertyName("path")]
    public string? path { get; set; }

    [JsonPropertyName("op")]
    public string? op
    {
        get
        {
            return _op;
        }
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

    public OperationBase() { }

    public OperationBase(string op, string path, string? from)
    {
        this.op = op ?? throw new ArgumentNullException(nameof(op));
        this.path = path ?? throw new ArgumentNullException(nameof(path));
        this.from = from;
    }

    public bool ShouldSerializefrom() => OperationType == OperationType.Move || OperationType == OperationType.Copy;
}
