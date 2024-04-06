using System.Collections;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Tingle.AspNetCore.JsonPatch.Internal;

namespace Tingle.AspNetCore.JsonPatch.Adapters;

/// <summary>
/// The default AdapterFactory to be used for resolving <see cref="IAdapter"/>.
/// </summary>
public class AdapterFactory : IAdapterFactory
{
    internal static AdapterFactory Default { get; } = new();

    /// <inheritdoc />
    public virtual IAdapter Create(object target, JsonSerializerOptions serializerOptions)
    {
        ArgumentNullException.ThrowIfNull(target);

        ArgumentNullException.ThrowIfNull(serializerOptions);

        if (target is JsonObject)
        {
            return new JsonObjectAdapter();
        }
        if (target is IList)
        {
            return new ListAdapter();
        }
        else if (target is IDictionary || target is IDictionary<string, object?>) // ExpandoObject implements IDictionary<string, object?>
        {
            var intf = target.GetType().GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            if (intf != null)
            {
                var type = typeof(DictionaryAdapter<,>).MakeGenericType(intf.GetGenericArguments());
                return (IAdapter)Activator.CreateInstance(type)!;
            }
        }
        else if (target is DynamicObject)
        {
            return new DynamicObjectAdapter();
        }

        return new PocoAdapter();
    }
}
