#if !NETCOREAPP
using System.Diagnostics;
#endif
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Tingle.Extensions.Http;

// heavily inspired by official JsonContent<TValue> bit it is internal
// https://github.com/dotnet/runtime/blob/e91db04be24aac28fd041425fac014ef04d940b1/src/libraries/System.Net.Http.Json/src/System/Net/Http/Json/JsonContentOfT.cs
// TODO: remove in .NET 8 (https://github.com/dotnet/runtime/issues/51544 is resolved)

internal class TingleJsonContent<TValue> : HttpContent
{
    private readonly JsonTypeInfo<TValue> jsonTypeInfo;
    private readonly TValue inputValue;

    public TingleJsonContent(TValue inputValue, JsonTypeInfo<TValue> jsonTypeInfo, MediaTypeHeaderValue? mediaType = null)
    {
        this.jsonTypeInfo = jsonTypeInfo ?? throw new ArgumentNullException(nameof(jsonTypeInfo));
        this.inputValue = inputValue;
        Headers.ContentType = mediaType ?? new("application/json") { CharSet = Encoding.UTF8.BodyName };
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        => SerializeToStreamAsyncCore(stream, async: true, CancellationToken.None);

#if NETCOREAPP

    protected override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
        => SerializeToStreamAsyncCore(stream, async: false, cancellationToken).GetAwaiter().GetResult();

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
        => SerializeToStreamAsyncCore(stream, async: true, cancellationToken);

#endif

    protected override bool TryComputeLength(out long length)
    {
        length = 0;
        return false;
    }

    private async Task SerializeToStreamAsyncCore(Stream targetStream, bool async, CancellationToken cancellationToken)
    {
        if (async)
        {
            await JsonSerializer.SerializeAsync(targetStream, inputValue, jsonTypeInfo, cancellationToken).ConfigureAwait(false);
        }
        else
        {
#if NETCOREAPP
            JsonSerializer.Serialize(targetStream, inputValue, jsonTypeInfo);
#else
            Debug.Fail("Synchronous serialization is only supported since .NET 5.0");
#endif
        }
    }
}

internal static class TingleJsonContent
{
    public static TingleJsonContent<TValue> Create<TValue>(TValue inputValue, JsonTypeInfo<TValue> jsonTypeInfo, MediaTypeHeaderValue? mediaType = null)
        => new(inputValue, jsonTypeInfo, mediaType);
}
