﻿using System.Reflection;

namespace Tingle.AspNetCore.OpenApi.ReDoc;

/// <summary>
/// Configuration options for use with <see cref="ReDocMiddleware"/>
/// </summary>
public class ReDocOptions
{
    /// <summary>
    /// The template for the OpenAPI document. Must include the {documentName} parameter.
    /// Defaults to '/openapi/{documentName}.json'.
    /// </summary>
    public string SpecUrlTemplate { get; set; } = "/openapi/{documentName}.json";

    /// <summary>
    /// The title of the page.
    /// Defaults to 'API Docs'.
    /// </summary>
    public string DocumentTitle { get; set; } = "API Docs";

    /// <summary>
    /// Gets or sets additional content to place in the head of the redoc page.
    /// Defaults to empty string
    /// </summary>
    public string HeadContent { get; set; } = "";

    /// <summary>
    /// The Url for the script.
    /// Defaults to the latest served via CDN
    /// </summary>
    public string ScriptUrl { get; set; } = "https://cdn.jsdelivr.net/npm/redoc/bundles/redoc.standalone.js";

    /// <summary>
    /// Configuration options for ReDoc UI
    /// </summary>
    public ReDocConfig Config { get; set; } = new ReDocConfig();

    /// <summary>
    /// Gets or sets a Stream function for retrieving the redoc page
    /// </summary>
    public Func<Stream> IndexStream { get; set; } = () => typeof(ReDocOptions).GetTypeInfo().Assembly
        .GetManifestResourceStream("Tingle.AspNetCore.OpenApi.ReDoc.index.html")!;
}
