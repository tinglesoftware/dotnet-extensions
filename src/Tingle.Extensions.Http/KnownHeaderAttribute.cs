namespace Tingle.Extensions.Http;

/// <summary>
/// Represents a known header
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class KnownHeaderAttribute : Attribute
{
    /// <summary>
    /// Creates an instance of <see cref="KnownHeaderAttribute"/>
    /// </summary>
    /// <param name="name">the name of the header</param>
    public KnownHeaderAttribute(string name) { Name = name; }

    /// <summary>
    /// The name of the header
    /// </summary>
    public string Name { get; set; }
}
