using System.Diagnostics.CodeAnalysis;

namespace Tingle.AspNetCore.JsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public class ConversionResult(bool canBeConverted, object? convertedInstance)
{
    [MemberNotNullWhen(true, nameof(ConvertedInstance))]
    public bool CanBeConverted { get; } = canBeConverted;
    public object? ConvertedInstance { get; } = convertedInstance;
}
