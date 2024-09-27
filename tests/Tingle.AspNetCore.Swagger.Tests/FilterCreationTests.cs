using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
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
    public void EnumDescriptionsFilters_AreAdded()
    {
        var env = new FakeWebHostEnvironment { ApplicationName = "Test", ContentRootPath = Environment.CurrentDirectory, };
        var services = new ServiceCollection().AddLogging()
                                              .AddSwaggerGen(o => o.IncludeXmlComments<EnumDescriptionsSchemaFilter>(true))
                                              .AddSwaggerEnumDescriptions()
                                              .BuildServiceProvider();

        var options = services.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;

        Assert.Single(options.SchemaFilterDescriptors, d => d.Type == typeof(EnumDescriptionsSchemaFilter));
    }

    [Fact]
    public void EnumDescriptionsFilter_CanBe_Created()
    {
        var env = new FakeWebHostEnvironment { ApplicationName = "Test", ContentRootPath = Environment.CurrentDirectory, };
        var services = new ServiceCollection().AddLogging()
                                              .AddSwaggerGen(o => o.IncludeXmlComments<EnumDescriptionsSchemaFilter>(true))
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
