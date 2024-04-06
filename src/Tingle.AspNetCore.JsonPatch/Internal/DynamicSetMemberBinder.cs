using System.Dynamic;

namespace Tingle.AspNetCore.JsonPatch.Internal;

internal class DynamicSetMemberBinder(string name, bool ignoreCase) : SetMemberBinder(name, ignoreCase)
{
    public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject? errorSuggestion)
    {
        throw new InvalidOperationException(typeof(DynamicSetMemberBinder).FullName + ".FallbackGetMember");
    }
}
