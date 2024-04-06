namespace Tingle.Extensions.Mustache.Parsing;

/// <summary>
/// The kind of token that is part of a template.
/// </summary>
public enum TemplateTokenKind
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    SingleValueEscaped,
    SingleValueUnescaped,
    ElementOpenInverted,
    ElementOpen,
    ElementClose,
    Comment,
    Content,
    CollectionOpen,
    CollectionClose,
    Custom,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
