namespace Tingle.Extensions.Mustache.Contexts;

/// <summary>
/// Allows us to capture how each path is used in an inferred template.
/// </summary>
public enum InferredUsage
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Scalar,
    ConditionalValue,
    Collection,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
