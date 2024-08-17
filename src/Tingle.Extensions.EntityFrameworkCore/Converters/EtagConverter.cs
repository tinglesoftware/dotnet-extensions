using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.EntityFrameworkCore.Converters;

///
public class EtagToBytesConverter : ValueConverter<Etag, byte[]>
{
    ///
    public EtagToBytesConverter() : base(convertToProviderExpression: v => v.ToByteArray(),
                                         convertFromProviderExpression: v => v == null ? default : new Etag(v))
    { }
}

///
public class EtagToInt32Converter : ValueConverter<Etag, int>
{
    ///
    public EtagToInt32Converter() : base(convertToProviderExpression: v => Convert.ToInt32((ulong)v),
                                         convertFromProviderExpression: v => new Etag(Convert.ToUInt64(v)))
    { }
}

///
public class EtagToUInt32Converter : ValueConverter<Etag, uint>
{
    ///
    public EtagToUInt32Converter() : base(convertToProviderExpression: v => Convert.ToUInt32((ulong)v),
                                          convertFromProviderExpression: v => new Etag(v))
    { }
}

///
public class EtagToInt64Converter : ValueConverter<Etag, long>
{
    ///
    public EtagToInt64Converter() : base(convertToProviderExpression: v => Convert.ToInt64((ulong)v),
                                         convertFromProviderExpression: v => new Etag(Convert.ToUInt64(v)))
    { }
}

///
public class EtagToUInt64Converter : ValueConverter<Etag, ulong>
{
    ///
    public EtagToUInt64Converter() : base(convertToProviderExpression: v => (ulong)v,
                                          convertFromProviderExpression: v => new Etag(v))
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
