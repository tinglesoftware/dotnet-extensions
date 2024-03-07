using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.EntityFrameworkCore.Converters;

///
public class SequenceNumberConverter : ValueConverter<SequenceNumber, long>
{
    ///
    public SequenceNumberConverter() : base(convertToProviderExpression: v => v.Value,
                                            convertFromProviderExpression: v => v == SequenceNumber.Empty ? default : new SequenceNumber(v))
    { }
}

///
public class SequenceNumberComparer : ValueComparer<SequenceNumber>
{
    ///
    public SequenceNumberComparer() : base(equalsExpression: (l, r) => l == r,
                                           hashCodeExpression: v => v.GetHashCode(),
                                           snapshotExpression: v => new SequenceNumber(v.Value))
    { }
}
