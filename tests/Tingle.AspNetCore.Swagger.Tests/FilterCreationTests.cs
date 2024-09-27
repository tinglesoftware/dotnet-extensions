using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tingle.AspNetCore.Swagger.Filters.Schemas;

namespace Tingle.AspNetCore.Swagger.Tests;

public class FilterCreationTests
{
    [Fact]
    public void InheritDocSchemaFilter_IsAdded_And_Created()
    {
        var services = CreateServiceProvider();

        var options1 = services.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;
        var descriptors = options1.SchemaFilterDescriptors.Where(sfd => sfd.Type == typeof(InheritDocSchemaFilter));
        var descriptor = Assert.Single(descriptors);
        var arguments = descriptor.Arguments;
        Assert.Equal(2, arguments.Length);
        Assert.IsType<SwaggerGenOptions>(arguments[0]);
        Assert.IsType<Type[]>(arguments[1]);

        var options2 = services.GetRequiredService<IOptions<SchemaGeneratorOptions>>().Value;
        Assert.Single(options2.SchemaFilters.OfType<InheritDocSchemaFilter>());
    }

    [Fact]
    public void EnumDescriptionsFilters_AreAdded_And_Created()
    {
        var services = CreateServiceProvider();

        var options1 = services.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;
        Assert.Single(options1.SchemaFilterDescriptors, d => d.Type == typeof(EnumDescriptionsSchemaFilter));

        var options2 = services.GetRequiredService<IOptions<SchemaGeneratorOptions>>().Value;
        Assert.Single(options2.SchemaFilters.OfType<EnumDescriptionsSchemaFilter>());
    }

    private static IServiceProvider CreateServiceProvider(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSwaggerGen(options =>
        {
            options.IncludeXmlComments<EnumDescriptionsSchemaFilter>(true);
            options.IncludeXmlCommentsFromInheritDocs();
        });
        services.AddSwaggerEnumDescriptions();
        configure?.Invoke(services);
        return services.BuildServiceProvider();
    }
}
