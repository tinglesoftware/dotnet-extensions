using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Tingle.Extensions.Caching.MongoDB;

internal class MongoCacheEntry
{
    [BsonId]
    public string? Key { get; set; }

    [BsonIgnoreIfNull]
    [BsonElement("content")]
    public byte[]? Content { get; set; }

    [BsonIgnoreIfNull]
    [BsonElement("ttl")]
    public long? TimeToLive { get; set; }

    [BsonIgnoreIfNull]
    [BsonElement("expires")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="TimeToLive"/> is sliding or absolute.<br/>
    /// True if <see cref="DistributedCacheEntryOptions.SlidingExpiration"/> was set and used for the TTL,
    /// false if <see cref="DistributedCacheEntryOptions.AbsoluteExpiration"/> or
    /// <see cref="DistributedCacheEntryOptions.AbsoluteExpirationRelativeToNow"/> was set and used for the TTL;
    /// otherwise null.
    /// </summary>
    [BsonIgnoreIfNull]
    [BsonElement("sliding")]
    public bool? IsSlidingExpiration { get; set; }
}
