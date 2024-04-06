using System.Text.Json;
using Tingle.AspNetCore.JsonPatch.Internal;

namespace Tingle.AspNetCore.JsonPatch.Adapters;

/// <summary>
/// Defines the operations used for loading an <see cref="IAdapter"/> based on the current object and ContractResolver.
/// </summary>
public interface IAdapterFactory
{
    /// <summary>
    /// Creates an <see cref="IAdapter"/> for the current object
    /// </summary>
    /// <param name="target">The target object</param>
    /// <param name="serializerOptions">The current <see cref="JsonSerializerOptions"/></param>
    /// <returns>The needed <see cref="IAdapter"/></returns>
    IAdapter Create(object target, JsonSerializerOptions serializerOptions);
}
