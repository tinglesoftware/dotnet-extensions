using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.EntityFrameworkCore.Converters;

///
public class ByteSizeConverter : ValueConverter<ByteSize, long>
{
    ///
    public ByteSizeConverter() : base(convertToProviderExpression: v => v.Bytes,
                                      convertFromProviderExpression: v => v == default ? default : new ByteSize(v))
    { }
}

///
public class ByteSizeComparer : ValueComparer<ByteSize>
{
    ///
    public ByteSizeComparer() : base(equalsExpression: (l, r) => l == r,
                                     hashCodeExpression: v => v.GetHashCode(),
                                     snapshotExpression: v => new ByteSize(v.Bytes))
    { }
}
