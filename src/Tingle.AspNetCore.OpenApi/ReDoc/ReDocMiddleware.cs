using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using SC = Tingle.AspNetCore.OpenApi.OpenApiJsonSerializerContext;

#pragma warning disable CS9113 // Parameter is unread.

namespace Tingle.AspNetCore.OpenApi.ReDoc;

internal class ReDocMiddleware(RequestDelegate _, IOptions<ReDocOptions> optionsAccessor)
{
    private readonly ReDocOptions options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

    public async Task Invoke(HttpContext httpContext)
    {
        var routeValues = httpContext.Request.RouteValues;

        // extract the documentName
        var documentName = routeValues.GetValueOrDefault("documentName")?.ToString();

        // formulate the URL for the spec (JSON or YAML)
        var specUrl = options.SpecUrlTemplate.Replace("{documentName}", documentName);

        // write the response
        var response = httpContext.Response;
        using var stream = options.IndexStream();
        // Inject arguments before writing to response
        var htmlBuilder = new StringBuilder(new StreamReader(stream).ReadToEnd());
        foreach (var entry in GetIndexArguments(specUrl))
        {
            htmlBuilder.Replace(entry.Key, entry.Value);
        }

        response.StatusCode = 200;
        response.ContentType = "text/html";
        await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8).ConfigureAwait(false);
    }

    private Dictionary<string, string> GetIndexArguments(string specUrl)
    {
        return new Dictionary<string, string>()
        {
            { "%(DocumentTitle)", options.DocumentTitle },
            { "%(HeadContent)", options.HeadContent },
            { "%(SpecUrl)", specUrl },
            { "%(ScriptUrl)", options.ScriptUrl },
            { "%(Config)", JsonSerializer.Serialize(options.Config, SC.Default.ReDocConfig) }
        };
    }
}
