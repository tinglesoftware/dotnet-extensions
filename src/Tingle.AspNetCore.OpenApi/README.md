# Tingle.AspNetCore.OpenApi

This library contains a bunch of extensions for OpenAPI document generation in ASP.NET Core.

## Getting started

To use the extensions, install [NuGet package](https://www.nuget.org/packages/Tingle.AspNetCore.OpenApi/) into your project.

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

Then configure OpenAPI to incorporate the error codes into the generated document. In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddOpenApi("v1", options =>
{
    options.AddErrorCodes();
})
```

You can optionally add descriptions to the error codes by supplying an instance of `IDictionary<string, string>` to `AddErrorCodes(...)`.

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

Then configure OpenApi to incorporate the tag groups into the generated document. In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;
using Tingle.AspNetCore.OpenApi;

var tagGroups = new new List<OpenApiTagGroup>
{
    new OpenApiTagGroup("Engineering", new List<string>
    {
        "Telemetry",
        "Devices"
    })
};

services.AddOpenApi("v1", options =>
{
    options.AddTagGroups(tagGroups);
})
```

By default, any tags that haven't been added to any group will be left as is. However, if you'd like to add them to a `ungrouped` grouping you can set the `addUngrouped` parameter in `AddTagGroups(...)` to `true`.

### Add description for Bad Request (400) Responses

In the `Program.cs` or `Startup.cs`:

```cs
using Microsoft.Extensions.DependencyInjection;

services.AddOpenApi("v1", options =>
{
    options.AlwaysShowBadRequestResponse();
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

services.AddOpenApi("v1", options =>
{
    options.AddInternalOnlyExtensions();
})
```

Then you can generate an external and internal version of the API. More details can be found [here](https://redocly.com/docs/cli/guides/hide-apis/#step-3-output-internal-and-external-apis).

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

services.AddOpenApi("v1", options =>
{
    options.AddReDocLogo();
})
```

When specified, the logo image is displayed above the navigation sidebar, on the left side of the API documentation page.
