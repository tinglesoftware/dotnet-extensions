using Microsoft.AspNetCore.Mvc.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc.ApiExplorer;

/// <summary>Extensions for <see cref="ApiDescription"/>.</summary>
public static class ApiDescriptionExtensions
{
    // Copied from https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/src/Swashbuckle.AspNetCore.SwaggerGen/SwaggerGenerator/ApiDescriptionExtensions.cs
    // Permalink: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/19c9dd2e20df1ce4cff21737deb8aed8f88f9c18/src/Swashbuckle.AspNetCore.SwaggerGen/SwaggerGenerator/ApiDescriptionExtensions.cs

    /// <summary>
    /// Attempts to get the <see cref="MethodInfo"/> for the action/endpoint described by the <paramref name="apiDescription"/>.
    /// </summary>
    /// <param name="apiDescription">The <see cref="ApiDescription"/> instance.</param>
    /// <param name="methodInfo">The <see cref="MethodInfo"/> if available.</param>
    /// <returns>
    /// <see langword="true"/> if a <see cref="MethodInfo"/> was found; otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryGetMethodInfo(this ApiDescription apiDescription, [NotNullWhen(true)] out MethodInfo? methodInfo)
    {
        if (apiDescription.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            methodInfo = controllerActionDescriptor.MethodInfo;
            return true;
        }

        if (apiDescription.ActionDescriptor?.EndpointMetadata != null)
        {
            methodInfo = apiDescription.ActionDescriptor.EndpointMetadata
                .OfType<MethodInfo>()
                .FirstOrDefault();

            return methodInfo != null;
        }

        methodInfo = null;
        return false;
    }
}
