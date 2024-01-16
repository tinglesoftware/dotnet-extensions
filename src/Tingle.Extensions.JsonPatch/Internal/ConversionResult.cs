namespace Tingle.Extensions.JsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public class ConversionResult(bool canBeConverted, object? convertedInstance)
{
    public bool CanBeConverted { get; } = canBeConverted;
    public object? ConvertedInstance { get; } = convertedInstance;
}
