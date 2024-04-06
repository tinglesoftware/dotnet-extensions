namespace Tingle.Extensions.Mustache.Parsing;

/// <summary>
/// The precedence that a given <see cref="TemplateTokenExpander"/> has.
/// </summary>
public enum TemplateTokenExpanderPrecedence
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Low,
    Medium,
    High,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
