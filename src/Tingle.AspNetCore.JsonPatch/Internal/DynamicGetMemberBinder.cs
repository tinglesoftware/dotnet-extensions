using System.Dynamic;

namespace Tingle.AspNetCore.JsonPatch.Internal;

internal class DynamicGetMemberBinder(string name, bool ignoreCase) : GetMemberBinder(name, ignoreCase)
{
    public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject? errorSuggestion)
    {
        throw new InvalidOperationException(typeof(DynamicGetMemberBinder).FullName + ".FallbackGetMember");
    }
}
