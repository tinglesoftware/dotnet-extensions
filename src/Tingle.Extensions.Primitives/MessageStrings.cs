namespace Tingle.Extensions.Primitives;

internal class MessageStrings
{
    public const string EnumMemberUnreferencedCodeMessage = "This overload uses reflection on the runtime enum type. Use the generic overload when trimming is required.";
    public const string EnumMemberRequiresDynamicCodeMessage = "This overload uses reflection on the runtime enum type. Use the generic overload when trimming is required.";
}
