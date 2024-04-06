using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tingle.AspNetCore.JsonPatch;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Extension methods for <see cref="IMvcBuilder"/>.</summary>
public static class IMvcBuilderExtensions
{
    /// <summary>
    /// Adds JSON Patch support via System.Text.Json library.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <returns>The <see cref="IMvcBuilder"/>.</returns>
    public static IMvcBuilder AddJsonPatch(this IMvcBuilder builder)
    {
        var services = builder.Services;

        services.TryAddEnumerable(
            ServiceDescriptor.Transient<IApiDescriptionProvider, JsonPatchOperationsArrayProvider>());

        services.TryAddEnumerable(
            ServiceDescriptor.Transient<IApiDescriptionProvider, JsonPatchMergeDocumentProvider>());

        services.TryAddEnumerable(
            ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, JsonPatchMvcOptionsSetup>());

        return builder;
    }

    private class JsonPatchMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly JsonOptions jsonOptions;

        public JsonPatchMvcOptionsSetup(ILoggerFactory loggerFactory, IOptions<JsonOptions> jsonOptions)
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.jsonOptions = jsonOptions?.Value ?? throw new ArgumentNullException(nameof(jsonOptions)); ;
        }

        public void Configure(MvcOptions options)
        {
            // Register patch input formatters before SystemTextJsonInputFormatter, otherwise
            // SystemTextJsonInputFormatter would consume "application/json-patch+json" requests first

            options.InputFormatters.Insert(
                0,
                new SystemTextJsonPatchInputFormatter(
                    jsonOptions,
                    loggerFactory.CreateLogger<SystemTextJsonPatchInputFormatter>()));

            options.InputFormatters.Insert(
                0,
                new SystemTextJsonPatchMergeInputFormatter(
                    jsonOptions,
                    loggerFactory.CreateLogger<SystemTextJsonPatchMergeInputFormatter>()));
        }
    }
}
