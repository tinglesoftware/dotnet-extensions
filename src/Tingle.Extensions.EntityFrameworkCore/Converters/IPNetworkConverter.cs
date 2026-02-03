using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net;

namespace Tingle.Extensions.EntityFrameworkCore.Converters;

///
public class IPNetworkConverter : ValueConverter<IPNetwork, string>
{
    ///
    public IPNetworkConverter() : base(convertToProviderExpression: v => v.ToString(),
                                       convertFromProviderExpression: v => v == null ? default : IPNetwork.Parse(v))
    { }
}

///
public class IPNetworkComparer : ValueComparer<IPNetwork>
{
    ///
    public IPNetworkComparer() : base(equalsExpression: (l, r) => l == r,
                                      hashCodeExpression: v => v.GetHashCode(),
                                      snapshotExpression: v => IPNetwork.Parse(v.ToString()))
    { }
}
