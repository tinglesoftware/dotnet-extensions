using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Tingle.AspNetCore.JsonPatch.Adapters;

namespace Tingle.AspNetCore.JsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
/// <param name="path">The path of the JsonPatch operation</param>
/// <param name="serializerOptions">The <see cref="JsonSerializerOptions"/>.</param>
/// <param name="adapterFactory">The <see cref="IAdapterFactory"/> to use when creating adaptors.</param>
public class ObjectVisitor(ParsedPath path, JsonSerializerOptions serializerOptions, IAdapterFactory adapterFactory, bool create)
{
    private readonly IAdapterFactory adapterFactory = adapterFactory ?? throw new ArgumentNullException(nameof(adapterFactory));
    private readonly JsonSerializerOptions serializerOptions = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));

    /// <summary>
    /// Initializes a new instance of <see cref="ObjectVisitor"/>.
    /// </summary>
    /// <param name="path">The path of the JsonPatch operation</param>
    /// <param name="serializerOptions">The <see cref="JsonSerializerOptions"/>.</param>
    public ObjectVisitor(ParsedPath path, JsonSerializerOptions serializerOptions, bool create)
        : this(path, serializerOptions, AdapterFactory.Default, create)
    {
    }

    public bool TryVisit(ref object target, [NotNullWhen(true)] out IAdapter? adapter, out string? errorMessage)
    {
        if (target == null)
        {
            adapter = null;
            errorMessage = null;
            return false;
        }

        adapter = SelectAdapter(target);

        // Traverse until the penultimate segment to get the target object and adapter
        for (var i = 0; i < path.Segments.Count - 1; i++)
        {
            if (!adapter.TryTraverse(target, path.Segments[i], serializerOptions, out var next, out errorMessage))
            {
                if (!create || !adapter.TryCreate(target, path.Segments[i], serializerOptions, out next, out errorMessage))
                {
                    adapter = null;
                    return false;
                }
            }

            // If we hit a null on an interior segment then we need to stop traversing.
            if (next == null)
            {
                adapter = null;
                return false;
            }

            target = next;
            adapter = SelectAdapter(target);
        }

        errorMessage = null;
        return true;
    }

    private IAdapter SelectAdapter(object targetObject)
    {
        return adapterFactory.Create(targetObject, serializerOptions);
    }
}
