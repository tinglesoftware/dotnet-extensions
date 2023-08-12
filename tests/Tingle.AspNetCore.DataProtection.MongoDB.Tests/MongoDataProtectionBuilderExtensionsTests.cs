using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Tingle.AspNetCore.DataProtection.MongoDB.Tests;

public class MongoDataProtectionBuilderExtensionsTests
{
    [Fact]
    public void PersistKeysToMongo_UsesMongoXmlRepository()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IMongoClient>(p => new MongoClient("mongodb://localhost:27017/services-management"));
        var builder = serviceCollection.AddDataProtection();

        // Act
        builder.PersistKeysToMongo();
        var services = serviceCollection.BuildServiceProvider();

        // Assert
        var options = services.GetRequiredService<IOptions<KeyManagementOptions>>();
        Assert.IsType<MongoXmlRepository>(options.Value.XmlRepository);
    }
}
