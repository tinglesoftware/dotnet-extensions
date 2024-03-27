# Tingle.Extensions.EntityFrameworkCore

## Converters

|Source Types|Destination Types|
|--|--|
|`System.Text.Json.Nodes.JsonObject`|`string`|
|`System.Text.Json.Nodes.JsonNode`|`string`|
|`System.Text.Json.JsonElement`|`string`|
|`System.Net.IPNetwork` (.NET 8 or later)|`String`|
|`Tingle.Extensions.Primitives.Etag`|`byte[]`|
|`Tingle.Extensions.Primitives.Duration`|`String`|
|`Tingle.Extensions.Primitives.ByteSize`|`long`|
|`Tingle.Extensions.Primitives.SequenceNumber`|`long`|

## Conventions

|Name|Description|
|--|--|
|`LengthAttributeConvention`|A convention that configures the maximum length based on the `LengthAttribute` applied on a property.|

## Database setup

For development and preview environments, databases may need to be migrated or created automatically on startup.
This can be done using `EFCORE_PERFORM_MIGRATIONS` or `EFCORE_CREATE_DATABASE` environment variable.

|Name|Description|
|--|--|
|EFCORE_PERFORM_MIGRATIONS|Whether to perform database migrations on startup.|
|EFCORE_CREATE_DATABASE|Whether to create the database on startup. Ignored if `EFCORE_PERFORM_MIGRATIONS` is set to `true` or `1`.|

```json
{
  "profiles": {
    "SerilogSample": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development",
        "EFCORE_PERFORM_MIGRATIONS": "true"
      }
    }
  }
}
```

In the application setup:

```cs
builder.Services.AddDatabaseSetup<MyContext>();
```
