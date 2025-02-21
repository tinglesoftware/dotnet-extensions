namespace Microsoft.AspNetCore.Mvc;

/// 
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class OperationErrorCodesAttribute(params string[] errors) : Attribute
{
    /// 
    public string[] Errors { get; set; } = errors;
}
