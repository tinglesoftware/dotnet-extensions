using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json.Nodes;

#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

namespace Tingle.Extensions.EntityFrameworkCore.Converters;

///
public class JsonNodeConverter : ValueConverter<JsonNode, string>
{
    ///
    public JsonNodeConverter() : base(convertToProviderExpression: v => v.ToJsonString(default),
                                      convertFromProviderExpression: v => v == null ? default : JsonNode.Parse(v, default, default))
    { }
}

///
public class JsonNodeComparer : ValueComparer<JsonNode>
{
    ///
    public JsonNodeComparer() : base(equalsExpression: (l, r) => (l == null ? null : l.ToJsonString(default)) == (r == null ? null : r.ToJsonString(default)),
                                     hashCodeExpression: v => v == null ? 0 : v.ToJsonString(default).GetHashCode(),
                                     snapshotExpression: v => JsonNode.Parse(v.ToJsonString(default), default, default))
    { }
}
