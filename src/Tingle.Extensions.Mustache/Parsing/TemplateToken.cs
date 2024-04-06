using Tingle.Extensions.Mustache.Rendering;

namespace Tingle.Extensions.Mustache.Parsing;

/// <summary>
/// A token that is part of a template.
/// </summary>
public readonly struct TemplateToken : IEquatable<TemplateToken>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="kind">Kind of token.</param>
    /// <param name="value">Value of the token.</param>
    /// <param name="renderer"></param>
    public TemplateToken(TemplateTokenKind kind, string value, ITemplateTokenRenderer? renderer = null)
    {
        if (string.IsNullOrEmpty(Value = value))
        {
            throw new ArgumentException($"'{nameof(value)}' cannot be null or whitespace.", nameof(value));
        }

        if ((Kind = kind) == TemplateTokenKind.Custom && renderer is null)
        {
            throw new ArgumentNullException(nameof(renderer), "A renderer must be supplied for a custom token.");
        }

        Renderer = renderer;
    }

    /// <summary>Kind of token.</summary>
    public TemplateTokenKind Kind { get; }

    /// <summary>Value of the token.</summary>
    public string Value { get; }

    /// <summary><see cref="ITemplateTokenRenderer"/> to use.</summary>
    public ITemplateTokenRenderer? Renderer { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is TemplateToken token && Equals(token);

    /// <inheritdoc/>
    public bool Equals(TemplateToken other) => Kind == other.Kind && Value == other.Value && Renderer == other.Renderer;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Kind, Value, Renderer);

    /// <inheritdoc/>
    public override string ToString() => $"{Kind}, {Value}";

    /// <inheritdoc/>
    public static bool operator ==(TemplateToken left, TemplateToken right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(TemplateToken left, TemplateToken right) => !(left == right);

    /// <summary>Generate a <see cref="TemplateToken"/> for <see cref="TemplateTokenKind.Content"/>.</summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static TemplateToken Content(string value) => new(TemplateTokenKind.Content, value);
}
