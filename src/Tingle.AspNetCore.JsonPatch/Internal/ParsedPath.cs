using System.Text;
using Tingle.AspNetCore.JsonPatch.Exceptions;
using Tingle.AspNetCore.JsonPatch.Properties;

namespace Tingle.AspNetCore.JsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public readonly struct ParsedPath
{
    private readonly string[] _segments;

    public ParsedPath(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        _segments = ParsePath(path);
    }

    public string? LastSegment
    {
        get
        {
            if (_segments == null || _segments.Length == 0)
            {
                return null;
            }

            return _segments[^1];
        }
    }

    public IReadOnlyList<string> Segments => _segments ?? [];

    private static string[] ParsePath(string path)
    {
        var strings = new List<string>();
        var sb = new StringBuilder(path.Length);

        for (var i = 0; i < path.Length; i++)
        {
            if (path[i] == '/')
            {
                if (sb.Length > 0)
                {
                    strings.Add(sb.ToString());
                    sb.Length = 0;
                }
            }
            else if (path[i] == '~')
            {
                ++i;
                if (i >= path.Length)
                {
                    throw new JsonPatchException(Resources.FormatInvalidValueForPath(path), null);
                }

                if (path[i] == '0')
                {
                    sb.Append('~');
                }
                else if (path[i] == '1')
                {
                    sb.Append('/');
                }
                else
                {
                    throw new JsonPatchException(Resources.FormatInvalidValueForPath(path), null);
                }
            }
            else
            {
                sb.Append(path[i]);
            }
        }

        if (sb.Length > 0)
        {
            strings.Add(sb.ToString());
        }

        return strings.ToArray();
    }
}
