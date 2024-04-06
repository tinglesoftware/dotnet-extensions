using Microsoft.Net.Http.Headers;

namespace Tingle.AspNetCore.JsonPatch;

internal static class MediaTypeHeaderValues
{
    public static readonly MediaTypeHeaderValue ApplicationJsonPatch
        = MediaTypeHeaderValue.Parse("application/json-patch+json").CopyAsReadOnly();

    public static readonly MediaTypeHeaderValue ApplicationJsonPatchMerge
        = MediaTypeHeaderValue.Parse("application/merge-patch+json").CopyAsReadOnly();
}
