using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using SC = Tingle.Extensions.EntityFrameworkCore.EfCoreJsonSerializerContext;

namespace Tingle.Extensions.EntityFrameworkCore.Converters;

///
public class JsonElementConverter : ValueConverter<JsonElement, string>
{
    ///
    public JsonElementConverter() : base(convertToProviderExpression: v => v.ToString(),
                                         convertFromProviderExpression: v => v == null ? default : JsonDocument.Parse(v, default).RootElement)
    { }
}

///
public class JsonElementComparer : ValueComparer<JsonElement>
{
    ///
    public JsonElementComparer() : base(
        equalsExpression: (l, r) => JsonSerializer.Serialize(l, SC.Default.JsonElement) == JsonSerializer.Serialize(r, SC.Default.JsonElement),
        hashCodeExpression: v => v.GetHashCode(),
        snapshotExpression: v => JsonDocument.Parse(JsonSerializer.Serialize(v, SC.Default.JsonElement), default).RootElement)
    { }
}
