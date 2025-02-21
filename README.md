# .NET and ASP.NET Core convenience functionality

[![NuGet](https://img.shields.io/nuget/v/Tingle.AspNetCore.Authentication.svg)](https://www.nuget.org/packages/Tingle.AspNetCore.Authentication/)
[![GitHub Workflow Status](https://github.com/tinglesoftware/dotnet-extensions/actions/workflows/build.yml/badge.svg)](https://github.com/tinglesoftware/dotnet-extensions/actions/workflows/build.yml)
[![Dependabot](https://badgen.net/badge/Dependabot/enabled/green?icon=dependabot)](https://dependabot.com/)
[![license](https://img.shields.io/github/license/tinglesoftware/dotnet-extensions.svg)](LICENSE)

This repository contains projects/libraries for adding useful functionality to .NET and ASP.NET Core when running real-world applications in production. We have been using these packages at [Tingle](https://tingle.software) for years and thought it would be better if we shared them.

## Packages

|Package|Description|
|--|--|
|[`Tingle.AspNetCore.Authentication`](https://www.nuget.org/packages/Tingle.AspNetCore.Authentication/)|Convenience authentication functionality such as pass through and pre-shared key authentication mechanisms. See [docs](./src/Tingle.AspNetCore.Authentication/README.md) and [sample](./samples/AuthenticationSample)|
|[`Tingle.AspNetCore.Authorization`](https://www.nuget.org/packages/Tingle.AspNetCore.Authorization/)|Additional authorization functionality such as handlers and requirements. See [docs](./src/Tingle.AspNetCore.Authorization/README.md) and [sample](./samples/AuthorizationSample)|
|[`Tingle.AspNetCore.DataProtection.MongoDB`](https://www.nuget.org/packages/Tingle.AspNetCore.DataProtection.MongoDB/)|Data Protection store in [MongoDB](https://mongodb.com) for ASP.NET Core. See [docs](./src/Tingle.AspNetCore.DataProtection.MongoDB/README.md) and [sample](./samples/DataProtectionMongoDBSample).|
|[`Tingle.AspNetCore.JsonPatch`](https://www.nuget.org/packages/Tingle.AspNetCore.JsonPatch/)|JSON Patch support for AspNetCore using System.Text.Json. See [docs](./src/Tingle.AspNetCore.JsonPatch/README.md).|
|[`Tingle.AspNetCore.JsonPatch.NewtonsoftJson`](https://www.nuget.org/packages/Tingle.AspNetCore.JsonPatch.NewtonsoftJson/)|Helpers for validation when working with JsonPatch in ASP.NET Core. See [docs](./src/Tingle.AspNetCore.JsonPatch.NewtonsoftJson/README.md) and [blog](https://maxwellweru.com/blog/2020-11-17-immutable-properties-with-json-patch-in-aspnet-core).|
|[`Tingle.AspNetCore.Tokens`](https://www.nuget.org/packages/Tingle.AspNetCore.Tokens/)|Support for generation of continuation tokens in ASP.NET Core with optional expiry. Useful for pagination, user invite tokens, expiring operation tokens, etc. This is availed through the `ContinuationToken<T>` and `TimedContinuationToken<T>` types. See [docs](./src/Tingle.AspNetCore.Tokens/README.md) and [sample](./samples/TokensSample).|
|[`Tingle.Extensions.Caching.MongoDB`](https://www.nuget.org/packages/Tingle.Extensions.Caching.MongoDB/)|Distributed caching implemented with [MongoDB](https://mongodb.com) on top of `IDistributedCache`, inspired by [CosmosCache](https://github.com/Azure/Microsoft.Extensions.Caching.Cosmos). See [docs](./src/Tingle.Extensions.Caching.MongoDB/README.md) and [sample](./samples/AspNetCoreSessionState)|
|[`Tingle.Extensions.DataAnnotations`](https://www.nuget.org/packages/Tingle.Extensions.DataAnnotations/)|Additional data validation attributes in the `System.ComponentModel.DataAnnotations` namespace. Some of this should have been present in the framework but are very specific to some use cases. For example `FiveStarRatingAttribute`. See [docs](./src/Tingle.Extensions.DataAnnotations/README.md).|
|[`Tingle.Extensions.EntityFrameworkCore`](https://www.nuget.org/packages/Tingle.Extensions.EntityFrameworkCore/)|Convenience functionality and extensions for working with EntityFrameworkCore. See [docs](./src/Tingle.Extensions.EntityFrameworkCore/README.md).|
|[`Tingle.Extensions.Http`](https://www.nuget.org/packages/Tingle.Extensions.Http/)|Lightweight abstraction around `HttpClient` which can be used to build custom client with response wrapping semantics. See [docs](./src/Tingle.Extensions.Http/README.md).|
|[`Tingle.Extensions.Http.Authentication`](https://www.nuget.org/packages/Tingle.Extensions.Http.Authentication/)|Authentication providers for use with `HttpClient` and includes support for DI via `Microsoft.Extensions.Http`. See [docs](./src/Tingle.Extensions.Http.Authentication/README.md) and [sample](./samples/HttpAuthenticationSample).|
|[`Tingle.Extensions.JsonPatch`](https://www.nuget.org/packages/Tingle.Extensions.JsonPatch/)|JSON Patch (RFC 6902) support for .NET to easily generate JSON Patch documents using `System.Text.Json` for client applications. See [docs](./src/Tingle.Extensions.JsonPatch/README.md).|
|[`Tingle.Extensions.MongoDB`](https://www.nuget.org/packages/Tingle.Extensions.MongoDB/)|Extensions for working with MongoDB. See [docs](./src/Tingle.Extensions.MongoDB/README.md) and [sample](./samples/MongoDBSample).|
|[`Tingle.Extensions.Mustache`](https://www.nuget.org/packages/Tingle.Extensions.Mustache/)|Basic mustache cache implementation in .NET built upon the good work offered in at <https://github.com/ActiveCampaign/mustachio>. See [docs](./src/Tingle.Extensions.Mustache/README.md).|
|[`Tingle.Extensions.PhoneValidators`](https://www.nuget.org/packages/Tingle.Extensions.PhoneValidators/)|Convenience for validation of phone numbers either via attributes or resolvable services. See [docs](./src/Tingle.Extensions.PhoneValidators/README.md).|
|[`Tingle.Extensions.Primitives`](https://www.nuget.org/packages/Tingle.Extensions.Primitives/)|Additional primitive types such as `Money`, `Currency`, `Duration`, `Keygen`, `Etag` etc. See [docs](./src/Tingle.Extensions.Primitives/README.md).|
|[`Tingle.Extensions.Processing`](https://www.nuget.org/packages/Tingle.Extensions.Processing/)|Helpers for making processing of bulk in memory tasks. See [docs](./src/Tingle.Extensions.Processing/README.md).|
|[`Tingle.Extensions.PushNotifications`](https://www.nuget.org/packages/Tingle.Extensions.PushNotifications/)|Clients for sending push notifications via FCM, APNs etc. See [docs](./src/Tingle.Extensions.PushNotifications/README.md).|

### Issues &amp; Comments

Please leave all comments, bugs, requests, and issues on the Issues page. We'll respond to your request ASAP!

### License

The Library is licensed under the [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form") license. Refer to the [LICENSE](./LICENSE) file for more information.
