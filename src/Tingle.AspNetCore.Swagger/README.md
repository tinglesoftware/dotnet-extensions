# Tingle.AspNetCore.Swagger

Swagger is a specification for documenting your API and specifies the format (URL, method, and representation) used to describe REST web services. This library is used to generate API documentation as well as add API documentation UI through ReDoc.

The library includes the `Swashbuckle.AspNetCore.SwaggerGen` library which is a Swagger generator that builds the `SwaggerDocument` objects directly from your routes, controllers, and models. It is combined with the Swagger endpoint middleware to automatically expose Swagger JSON.

Swashbuckle relies heavily on `ApiExplorer` (the API metadata layer that ships with ASP.NET Core). If using `AddMvcCore` for a more pared down MVC stack, you'll need to explicitly add the `ApiExplorer` service.

In `ConfigureServices` method of `Program.cs`, add `MvcCore` service, the `ApiExplorer` service, and register the Swagger generator defining one or more Swagger documents.

```cs
// Add swagger documents generation
builder.Services.AddSwaggerGen(options =>
{
    options
        .AddDocuments(services.BuildServiceProvider(), ConstStrings.ApiDocsTitle, System.IO.File.ReadAllText("apidesc.md"))
        .AlwaysShowAuthorizationFailedResponse()
        .AlwaysShowBadRequestResponse();

    options.IncludeXmlComments<Program>();
});
```

In the `Configure` method of `Program.cs`, insert middleware to expose the generated Swagger as JSON endpoints.

```cs
// Add swagger schema docs
app.MapSwagger(options => options.PreSerializeFilters.Add((swaggerDoc, httpRequest) => swaggerDoc.Host = httpRequest.Host.Value));
```

In the code snippet above, the `SwaggerDocument` and the current `HttpRequest` are passed to the filter thus providing a lot of flexibility. The filter will be executed prior to serializing the document.

ReDoc functionalities are described in the sections below:

## ReDoc

ReDoc adds API documentation UI. It can be used to document complex request/response payloads. It also supports nested schemas and displays them in place with the ability to collapse or expand.

This library is used to add ReDoc middleware to the HTTP request pipeline. This adds API documentation UI.

## Setup

Add the following code logic to `Program.cs`

```cs
// Add ReDoc services
builder.Services.AddReDoc(o =>
{
    o.DocumentTitle = ConstStrings.ApiDocsTitle;
    o.DefaultDocumentName = $"v{ConstStrings.ApiVersion2}";
});

var app = builder.Build();

// Add API documentation UI via ReDoc
app.MapReDoc();
```
