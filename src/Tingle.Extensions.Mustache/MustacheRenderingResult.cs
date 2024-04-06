namespace Tingle.Extensions.Mustache;

/// <summary>
/// Result for rending of a Mustache template.
/// </summary>
public readonly struct MustacheRenderingResult : IEquatable<MustacheRenderingResult>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rendered"></param>
    /// <param name="usedModel"></param>
    public MustacheRenderingResult(string rendered, IReadOnlyDictionary<string, object?>? usedModel = null)
    {
        if (string.IsNullOrWhiteSpace(Rendered = rendered))
        {
            throw new ArgumentException($"'{nameof(rendered)}' cannot be null or whitespace.", nameof(rendered));
        }

        UsedModel = usedModel;
    }

    /// <summary>
    /// The rendered string.
    /// </summary>
    public string Rendered { get; }

    /// <summary>
    /// The model used for rendering.
    /// </summary>
    public IReadOnlyDictionary<string, object?>? UsedModel { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is MustacheRenderingResult result && Equals(result);

    /// <inheritdoc/>
    public bool Equals(MustacheRenderingResult other) => Rendered == other.Rendered;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Rendered, UsedModel);

    /// <inheritdoc/>
    public override string ToString() => Rendered;

    /// <inheritdoc/>
    public static bool operator ==(MustacheRenderingResult left, MustacheRenderingResult right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(MustacheRenderingResult left, MustacheRenderingResult right) => !(left == right);

    /// <summary>
    /// Converts a <see cref="MustacheRenderingResult"/> to an instance of <see cref=" string"/>;
    /// </summary>
    /// <param name="id">The <see cref="MustacheRenderingResult"/> to be converted.</param>
    public static implicit operator string(MustacheRenderingResult id) => id.ToString();
}
