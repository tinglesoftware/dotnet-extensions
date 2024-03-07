using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.EntityFrameworkCore.Converters;

///
public class DurationConverter : ValueConverter<Duration, string>
{
    ///
    public DurationConverter() : base(convertToProviderExpression: v => v.ToString(),
                                      convertFromProviderExpression: v => v == null ? default : Duration.Parse(v))
    { }
}

///
public class DurationComparer : ValueComparer<Duration>
{
    ///
    public DurationComparer() : base(equalsExpression: (l, r) => l == r,
                                     hashCodeExpression: v => v.GetHashCode(),
                                     snapshotExpression: v => Duration.Parse(v.ToString()))
    { }
}
