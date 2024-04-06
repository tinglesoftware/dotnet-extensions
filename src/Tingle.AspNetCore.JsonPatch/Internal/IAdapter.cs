using System.Text.Json;

namespace Tingle.AspNetCore.JsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public interface IAdapter
{
    bool TryTraverse(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out object? nextTarget,
        out string? errorMessage);

    bool TryCreate(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out object? nextTarget,
        out string? errorMessage);

    bool TryAdd(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        object? value,
        out string? errorMessage);

    bool TryRemove(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out string? errorMessage);

    bool TryGet(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out object? value,
        out string? errorMessage);

    bool TryReplace(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        object? value,
        out string? errorMessage);

    bool TryTest(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        object? value,
        out string? errorMessage);
}
