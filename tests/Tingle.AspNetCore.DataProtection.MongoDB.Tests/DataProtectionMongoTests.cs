using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Xml.Linq;
using Xunit;

namespace Tingle.AspNetCore.DataProtection.MongoDB.Tests;

public class DataProtectionMongoTests
{
    private const string CollName = "DataProtection-Keys";

    [Fact]
    public void StoreElement_PersistsData()
    {
        var element = XElement.Parse("<Element1/>");
        var friendlyName = "Element1";
        var key = new DataProtectionKey() { FriendlyName = friendlyName, Xml = element.ToString() };

        using var dbFixture = new MongoDbFixture();
        var serviceProvider = GetServiceProvider(dbFixture);
        var repo = GetRepo(serviceProvider);
        repo.StoreElement(element, friendlyName);

        var collection = dbFixture.GetCollection<DataProtectionKey>(CollName);
        var dbKeys = collection.Find(Builders<DataProtectionKey>.Filter.Empty).ToList();
        var dbKey = Assert.Single(dbKeys);
        Assert.Equal(key.FriendlyName, dbKey?.FriendlyName);
        Assert.Equal(key.Xml, dbKey?.Xml);
    }

    [Fact]
    public void GetAllElements_ReturnsAllElements()
    {
        var element1 = XElement.Parse("<Element1/>");
        var element2 = XElement.Parse("<Element2/>");

        using var dbFixture = new MongoDbFixture();
        var serviceProvider = GetServiceProvider(dbFixture);
        var repo1 = GetRepo(serviceProvider);
        repo1.StoreElement(element1, "element1");
        repo1.StoreElement(element2, "element2");

        // Use a separate instance of the context to verify correct data was saved to database
        var repo2 = GetRepo(serviceProvider);
        var elements = repo2.GetAllElements();
        Assert.Equal(2, elements.Count);
    }

    private MongoXmlRepository GetRepo(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<KeyManagementOptions>>();
        return Assert.IsType<MongoXmlRepository>(options.Value.XmlRepository);
    }

    private IServiceProvider GetServiceProvider(MongoDbFixture dbFixture)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMongoClient>(p => dbFixture.Client);

        services.AddDataProtection().PersistKeysToMongo(p => dbFixture.GetCollection<DataProtectionKey>(CollName));

        return services.BuildServiceProvider(validateScopes: true);
    }
}
