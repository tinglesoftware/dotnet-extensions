using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tingle.AspNetCore.Swagger.Filters.Documents;
using Tingle.AspNetCore.Swagger.Filters.Operations;
using Tingle.AspNetCore.Swagger.Filters.Parameters;
using Tingle.AspNetCore.Swagger.Filters.RequestBodies;
using Tingle.AspNetCore.Swagger.Filters.Schemas;

namespace Tingle.AspNetCore.Swagger.Tests;

public class FilterCreationTests
{
    [Fact]
    public void InheritDocSchemaFilter_IsAdded()
    {
        var services = new ServiceCollection().AddLogging()
                                              .AddSwaggerGen(o => o.IncludeXmlCommentsFromInheritDocs())
                                              .BuildServiceProvider();

        var options = services.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;
        var descriptors = options.SchemaFilterDescriptors.Where(sfd => sfd.Type == typeof(InheritDocSchemaFilter));
        var descriptor = Assert.Single(descriptors);
        var arguments = descriptor.Arguments;
        Assert.Equal(2, arguments.Length);
        Assert.IsType<SwaggerGenOptions>(arguments[0]);
        Assert.IsType<Type[]>(arguments[1]);
    }

    [Fact]
    public void InheritDocSchemaFilter_CanBe_Created()
    {
        var services = new ServiceCollection().AddLogging()
                                              .AddSwaggerGen(o => o.IncludeXmlCommentsFromInheritDocs())
                                              .BuildServiceProvider();

        var options = services.GetRequiredService<IOptions<SchemaGeneratorOptions>>().Value;
        Assert.Single(options.SchemaFilters.OfType<InheritDocSchemaFilter>());
    }

    [Fact]
    public void MarkdownFilters_AreAdded()
    {
        var env = new FakeWebHostEnvironment { ApplicationName = "Test", ContentRootPath = Environment.CurrentDirectory, };
        var services = new ServiceCollection().AddLogging()
                                              .AddSwaggerGen(o => o.IncludeXmlComments<MarkdownDocumentFilter>(env, includeControllerXmlComments: true))
                                              .AddSwaggerXmlToMarkdown()
                                              .BuildServiceProvider();

        var options = services.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;

        Assert.Single(options.DocumentFilterDescriptors.Where(d => d.Type == typeof(MarkdownDocumentFilter)));
        Assert.Single(options.OperationFilterDescriptors.Where(d => d.Type == typeof(MarkdownOperationFilter)));
        Assert.Single(options.ParameterFilterDescriptors.Where(d => d.Type == typeof(MarkdownParameterFilter)));
        Assert.Single(options.RequestBodyFilterDescriptors.Where(d => d.Type == typeof(MarkdownRequestBodyFilter)));
        Assert.Single(options.SchemaFilterDescriptors.Where(d => d.Type == typeof(MarkdownSchemaFilter)));
    }

    [Fact]
    public void MarkdownFilters_CanBe_Created()
    {
        var env = new FakeWebHostEnvironment { ApplicationName = "Test", ContentRootPath = Environment.CurrentDirectory, };
        var services = new ServiceCollection().AddLogging()
                                              .AddSwaggerGen(o => o.IncludeXmlComments<MarkdownDocumentFilter>(env, includeControllerXmlComments: true))
                                              .AddSwaggerXmlToMarkdown()
                                              .AddSingleton<IWebHostEnvironment>(env)
                                              .BuildServiceProvider();

        var optionsSchema = services.GetRequiredService<IOptions<SchemaGeneratorOptions>>().Value;
        Assert.Single(optionsSchema.SchemaFilters.OfType<MarkdownSchemaFilter>());

        var options = services.GetRequiredService<IOptions<SwaggerGeneratorOptions>>().Value;
        Assert.Single(options.DocumentFilters.OfType<MarkdownDocumentFilter>());
        Assert.Single(options.OperationFilters.OfType<MarkdownOperationFilter>());
        Assert.Single(options.ParameterFilters.OfType<MarkdownParameterFilter>());
        Assert.Single(options.RequestBodyFilters.OfType<MarkdownRequestBodyFilter>());
    }

    [Fact]
    public void EnumDescriptionsFilters_AreAdded()
    {
        var env = new FakeWebHostEnvironment { ApplicationName = "Test", ContentRootPath = Environment.CurrentDirectory, };
        var services = new ServiceCollection().AddLogging()
                                              .AddSwaggerGen(o => o.IncludeXmlComments<MarkdownDocumentFilter>(env, includeControllerXmlComments: true))
                                              .AddSwaggerEnumDescriptions()
                                              .BuildServiceProvider();

        var options = services.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;

        Assert.Single(options.SchemaFilterDescriptors.Where(d => d.Type == typeof(EnumDescriptionsSchemaFilter)));
    }

    [Fact]
    public void EnumDescriptionsFilter_CanBe_Created()
    {
        var env = new FakeWebHostEnvironment { ApplicationName = "Test", ContentRootPath = Environment.CurrentDirectory, };
        var services = new ServiceCollection().AddLogging()
                                              .AddSwaggerGen(o => o.IncludeXmlComments<MarkdownDocumentFilter>(env, includeControllerXmlComments: true))
                                              .AddSwaggerEnumDescriptions()
                                              .BuildServiceProvider();

        var options = services.GetRequiredService<IOptions<SchemaGeneratorOptions>>().Value;
        Assert.Single(options.SchemaFilters.OfType<EnumDescriptionsSchemaFilter>());
    }

    private class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = default!;
        public IFileProvider WebRootFileProvider { get; set; } = default!;
        public string ApplicationName { get; set; } = default!;
        public IFileProvider ContentRootFileProvider { get; set; } = default!;
        public string ContentRootPath { get; set; } = default!;
        public string EnvironmentName { get; set; } = default!;
    }
}
