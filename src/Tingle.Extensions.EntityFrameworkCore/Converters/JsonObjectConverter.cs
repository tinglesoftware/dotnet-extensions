using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

namespace Tingle.Extensions.EntityFrameworkCore.Converters;

///
public class JsonObjectConverter : ValueConverter<JsonObject, string>
{
    ///
    public JsonObjectConverter() : base(convertToProviderExpression: v => v.ToJsonString(default),
                                        convertFromProviderExpression: v => v == null ? default : JsonNode.Parse(v, default, default)!.AsObject())
    { }
}

///
[RequiresDynamicCode(MessageStrings.JsonComparisonRequiresDynamicCodeMessage)]
[RequiresUnreferencedCode(MessageStrings.JsonComparisonRequiresDynamicCodeMessage)]
public class JsonObjectComparer : ValueComparer<JsonObject>
{
    ///
    public JsonObjectComparer() : base(equalsExpression: (l, r) => (l == null ? null : l.ToJsonString(default)) == (r == null ? null : r.ToJsonString(default)),
                                       hashCodeExpression: v => v == null ? 0 : v.ToJsonString(default).GetHashCode(),
                                       snapshotExpression: v => v == null ? null : JsonNode.Parse(v.ToJsonString(default), default, default)!.AsObject())
    { }
}
