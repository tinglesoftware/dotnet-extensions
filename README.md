# .NET and ASP.NET Core convenience functionality

![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/tinglesoftware/dotnet-extensions/build-release.yml?branch=main&style=flat-square)

This repository contains projects/libraries for adding useful functionality to dotnet and ASP.NET Core when running real world applications in production. We have been using this packages at [Tingle](https://tingle.software) for years and thought it is better if we shared them.

## Packages

|Package|Version|Description|
|--|--|--|
|`Tingle.AspNetCore.Authorization`|[![NuGet](https://img.shields.io/nuget/v/Tingle.AspNetCore.Authorization.svg)](https://www.nuget.org/packages/Tingle.AspNetCore.Authorization/)|Additional authorization functionality such as handlers and requirements. See [docs](./src/Tingle.AspNetCore.Authorization/README.md) and [sample](./samples/AuthorizationSample)|
|`Tingle.AspNetCore.DataProtection.MongoDB`|[![NuGet](https://img.shields.io/nuget/v/Tingle.AspNetCore.DataProtection.MongoDB.svg)](https://www.nuget.org/packages/Tingle.AspNetCore.DataProtection.MongoDB/)|Data Protection store in [MongoDB](https://mongodb.com) for ASP.NET Core. See [docs](./src/Tingle.AspNetCore.DataProtection.MongoDB/README.md) and [sample](./samples/DataProtectionMongoDBSample).|
|`Tingle.AspNetCore.JsonPatch.NewtonsoftJson`|[![NuGet](https://img.shields.io/nuget/v/Tingle.AspNetCore.JsonPatch.NewtonsoftJson.svg)](https://www.nuget.org/packages/Tingle.AspNetCore.JsonPatch.NewtonsoftJson/)|Helpers for validation when working with JsonPatch in ASP.NET Core. See [docs](./src/Tingle.AspNetCore.JsonPatch.NewtonsoftJson/README.md) and [blog](https://medium.com/swlh/immutable-properties-with-json-patch-in-aspnet-core-25185f493ea8).|
|`Tingle.AspNetCore.Tokens`|[![NuGet](https://img.shields.io/nuget/v/Tingle.AspNetCore.Tokens.svg)](https://www.nuget.org/packages/Tingle.AspNetCore.Tokens/)|Support for generation of coninuation tokens in ASP.NET Core with optional expiry. Useful for pagination, user invite tokens, expiring operation tokens, etc. This is availed through the `ContinuationToken<T>` and `TimedContinuationToken<T>` types. See [docs](./src/Tingle.AspNetCore.Tokens/README.md) and [sample](./samples/TokensSample).|
|`Tingle.Extensions.Caching.MongoDB`|[![NuGet](https://img.shields.io/nuget/v/Tingle.Extensions.Caching.MongoDB.svg)](https://www.nuget.org/packages/Tingle.Extensions.Caching.MongoDB/)|Distributed caching implemented with [MongoDB](https://mongodb.com) on top of `IDistributedCache`, inspired by [CosmosCache](https://github.com/Azure/Microsoft.Extensions.Caching.Cosmos). See [docs](./src/Tingle.Extensions.Caching.MongoDB/README.md)and [sample](./samples/AspNetCoreSessionState)|
|`Tingle.Extensions.DataAnnotations`|[![NuGet](https://img.shields.io/nuget/v/Tingle.Extensions.DataAnnotations.svg)](https://www.nuget.org/packages/Tingle.Extensions.DataAnnotations/)|Additional data validation attributes in the `System.ComponentModel.DataAnnotations` namespace. Some of this should have been present in the framework but are very specific to some use cases. For example `FiveStarRatingAttribute`. See [docs](./src/Tingle.Extensions.DataAnnotations/README.md).|
|`Tingle.Extensions.Http.Authentication`|[![NuGet](https://img.shields.io/nuget/v/Tingle.Extensions.Http.Authentication.svg)](https://www.nuget.org/packages/Tingle.Extensions.Http.Authentication/)|Authentication providers for use with HttpClient and includes support for DI via `Microsoft.Extensions.Http`. See [docs](./src/Tingle.Extensions.Http.Authentication/README.md) and [sample](./samples/HttpAuthenticationSample).|
|`Tingle.Extensions.JsonPatch`|[![NuGet](https://img.shields.io/nuget/v/Tingle.Extensions.JsonPatch.svg)](https://www.nuget.org/packages/Tingle.Extensions.JsonPatch/)|JSON Patch (RFC 6902) support for .NET to easily generate JSON Patch documents using `System.Text.Json`. See [docs](./src/Tingle.Extensions.JsonPatch/README.md).|
|`Tingle.Extensions.PhoneValidators`|[![NuGet](https://img.shields.io/nuget/v/Tingle.Extensions.PhoneValidators.svg)](https://www.nuget.org/packages/Tingle.Extensions.PhoneValidators/)|Convenience for validation of phone numbers either via attributes or resolvable services. See [docs](./src/Tingle.Extensions.PhoneValidators/README.md).|
|`Tingle.Extensions.Processing`|[![NuGet](https://img.shields.io/nuget/v/Tingle.Extensions.Processing.svg)](https://www.nuget.org/packages/Tingle.Extensions.Processing/)|Helpers for making processing of bulk in memory tasks. See [docs](./src/Tingle.Extensions.Processing/README.md).|

### Issues &amp; Comments

Please leave all comments, bugs, requests, and issues on the Issues page. We'll respond to your request ASAP!

### License

The Library is licensed under the [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form") license. Refer to the [LICENSE](./LICENSE) file for more information.
