using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.EntityFrameworkCore.Converters;

///
public class EtagConverter : ValueConverter<Etag, byte[]>
{
    ///
    public EtagConverter() : base(convertToProviderExpression: v => v.ToByteArray(),
                                  convertFromProviderExpression: v => v == null ? default : new Etag(v))
    { }
}

///
public class EtagComparer : ValueComparer<Etag>
{
    ///
    public EtagComparer() : base(equalsExpression: (l, r) => l == r,
                                 hashCodeExpression: v => v.GetHashCode(),
                                 snapshotExpression: v => new Etag(v.ToByteArray()))
    { }
}
