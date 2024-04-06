namespace Tingle.Extensions.Mustache.Parsing;

/// <summary>
/// Represents the position for parsing
/// </summary>
/// <param name="line">Line number in the text.</param>
/// <param name="character">Character position in the <paramref name="line"/>.</param>
public readonly struct CharacterLocation(int line, int character) : IEquatable<CharacterLocation>
{
    /// <summary>Line number in the text.</summary>
    public int Line { get; } = line;

    /// <summary>Character position in the <see cref="Line"/>.</summary>
    public int Character { get; } = character;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is CharacterLocation location && Equals(location);

    /// <inheritdoc/>
    public bool Equals(CharacterLocation other) => Line == other.Line && Character == other.Character;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Line, Character);

    /// <inheritdoc/>
    public override string ToString() => $"{Line}:{Character}";

    /// <inheritdoc/>
    public static bool operator ==(CharacterLocation left, CharacterLocation right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(CharacterLocation left, CharacterLocation right) => !(left == right);
}
