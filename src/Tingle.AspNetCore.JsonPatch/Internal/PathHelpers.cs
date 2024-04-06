using Tingle.AspNetCore.JsonPatch.Exceptions;
using Tingle.AspNetCore.JsonPatch.Properties;

namespace Tingle.AspNetCore.JsonPatch.Helpers;

internal static class PathHelpers
{
    internal static string ValidateAndNormalizePath(string path)
    {
        // check for most common path errors on create.  This is not
        // absolutely necessary, but it allows us to already catch mistakes
        // on creation of the patch document rather than on execute.

        if (path.Contains("//"))
        {
            throw new JsonPatchException(Resources.FormatInvalidValueForPath(path), null);
        }

        if (!path.StartsWith('/'))
        {
            return "/" + path;
        }
        else
        {
            return path;
        }
    }
}
