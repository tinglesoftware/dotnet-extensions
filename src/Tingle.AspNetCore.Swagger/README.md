# Tingle.AspNetCore.Swagger

> [!IMPORTANT]
> Use of `Microsoft.AspNetCore.OpenApi` is the recommended replacement. This library is provided for convenience and compatibility with existing applications but is no longer actively maintained and not recommended for new projects.
> Supported extensions for `Microsoft.AspNetCore.OpenApi` are at [`Tingle.AspNetCore.OpenApi`](../Tingle.AspNetCore.OpenApi/README.md)

This library contains a bunch of extensions for [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).

## Getting started

To use the extensions, install [NuGet package](https://www.nuget.org/packages/Tingle.AspNetCore.Swagger/) into your project.

Then use whichever annotations or extensions (filters) you need.

## Extensions Use

### Add error codes

Annotate your controller action methods with the `OperationErrorCodes` attribute to indicate which error codes can be expected to be returned:

```cs
using Microsoft.AspNetCore.Mvc;

[HttpGet("{id}")]
[OperationErrorCodes("user_not_found", "user_ineligible_for_account")]
public async Task<IActionResult> CreateAccountAsync([FromRoute, Required] string id){ }
```

Then configure Swashbuckle to incorporate the error codes into the generated swagger documentation. In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddSwaggerGen(options =>
{
    options.AddErrorCodes();
})
```

You can optionally add descriptions to the error codes by supplying an instance of `IDictionary<string, string>` to `AddErrorCodes(...)`.

### Add extra tags

You can use tags to provide additional metadata to operations. You can do this by annotating your controllers or action methods with the `OperationExtraTag` attribute:

```cs
using Microsoft.AspNetCore.Mvc;

[Route("/devices")]
[OperationExtraTag("Devices", Description = "Accessing Device Operations", ExternalDocsUrl = "https://redocly.github.io/redoc/example-logo.png")]
public class DevicesController : ControllerBase { }
```

Then configure Swashbuckle to incorporate the extra tags into the generated swagger documentation. In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddSwaggerGen(options =>
{
    options.AddExtraTags();
})
```

You can further control the tags by [grouping them](#add-tag-groups).

### Add tag groups

Use `x-tagGroups` to group tags in the Reference docs navigation sidebar.

Add controllers you wish to group:

```cs
using Microsoft.AspNetCore.Mvc;

[Route("/devices")]
public class DevicesController : ControllerBase { }

[Route("/telemetry")]
public class TelemetryController : ControllerBase { }
```

Then configure Swashbuckle to incorporate the tag groups into the generated swagger documentation. In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;
using Tingle.AspNetCore.Swagger;

var tagGroups = new new List<OpenApiTagGroup>
{
    new OpenApiTagGroup("Engineering", new List<string>
    {
        "Telemetry",
        "Devices"
    })
};

services.AddSwaggerGen(options =>
{
    options.AddTagGroups(tagGroups);
})
```

By default, any tags that haven't been added to any group will be left as is. However, if you'd like to add them to a `ungrouped` grouping you can set the `addUngrouped` parameter in `AddTagGroups(...)` to `true`.

### Add description for Bad Request (400) Responses

In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddSwaggerGen(options =>
{
    options.AlwaysShowBadRequestResponse();
})
```

### Add description for Unauthorized (401) and Forbidden (403) Responses

In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddSwaggerGen(options =>
{
    options.AlwaysShowAuthorizationFailedResponse();
})
```

### Hide your internal APIs

Sometimes you may want to hide your endpoints internal use only. You can do this by annotating your controllers or action methods with the `InternalOnly` attribute:

```cs
using Microsoft.AspNetCore.Mvc;

[InternalOnly]
[Route("/devices")]
public class DevicesController : ControllerBase { }
```

Then you can add the `x-internal` to the API description. In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddSwaggerGen(options =>
{
    options.AddInternalOnlyExtensions();
})
```

Then you can generate an external and internal version of the API. More details can be found [here](https://redocly.com/docs/cli/guides/hide-apis/#step-3-output-internal-and-external-apis).

### Add Correlation ID headers to all operations

You can add the `X-Correlation-ID` headers to all operations, that are used to uniquely identify the HTTP request, as shown below:

In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddSwaggerGen(options =>
{
    options.AddCorrelationIds();
})
```

By default, the header is only indicated to be part of the response headers. If you'd like the header to appear among the request headers you can set the `includeInRequests` parameter in `AddCorrelationIds(...)` to `true`.

### Add Logo

Use `x-logo` to add a custom logo image to your API reference documentation. For example:

In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

var logo = new OpenApiReDocLogo
{
    Url = "https://redocly.github.io/redoc/example-logo.png",
    BackgroundColor = "#FFFFFF",
    AltText = "Example logo"
};

services.AddSwaggerGen(options =>
{
    options.AddReDocLogo();
})
```

When specified, the logo image is displayed above the navigation sidebar, on the left side of the API documentation page.

### Add swagger documents from API version descriptions

You can automatically discover API versions declared in code and generate swagger documents as shown below:

In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddSwaggerGen(options =>{});
services.AddSwaggerDocsAutoDiscovery(title: "My docs", description: "My description", skipDeprecated: false, deprecationSuffix: "[deprecated]");
```

If you'd like to add swagger docs for only particular API versions.

Annotate a controller with an API version:

```cs
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("v1")]
[Route("/devices")]
public class DevicesController : ControllerBase { }
```

In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddSwaggerGen(options =>
{
    options.AddDocument(documentName: "v1", versionName: "v1", title: "My v1 docs", description: "My v1 docs");
});
```

You can use the various `AddDocument(...)` or `AddDocuments(...)` overloads to also achieve the same result.

### Add conversion of XML comments extracted for Swagger to markdown

In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddSwaggerGen(options => { });
services.AddSwaggerXmlToMarkdown();
```

### Add enum descriptions

In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddSwaggerGen(options => { });
services.AddSwaggerEnumDescriptions();
```

This should be called after all XML documents have been added.

### Add XML comments from summary and remarks into the swagger documentation

In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddSwaggerGen(options =>
{
    options.IncludeXmlCommentsFromInheritDocs();
});
```

You can alternatively use the `IncludeXmlComments(...)` extension to add XML comments from a file in a certain directory or from the assembly of the specified type.
