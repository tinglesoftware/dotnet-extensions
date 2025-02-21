namespace Microsoft.AspNetCore.Mvc;

/// 
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class OperationExtraTagAttribute(string name) : Attribute
{
    /// 
    public string Name { get; set; } = name ?? throw new ArgumentNullException(nameof(name));

    /// 
    public string? Description { get; set; }

    /// 
    public string? ExternalDocsUrl { get; set; }
}
