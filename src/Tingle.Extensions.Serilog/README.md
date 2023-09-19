# Tingle.Extensions.Serilog

This library provides convenience extensions and logic for registering Serilog in applications with support for various hosts while setting up basic thins to match the default logging setup in the framework.

This includes:

- Destructuring is enabled/added by default
- Sensitive data masking is enabled/added by default.
- Console and Debug are registered by default.
- Enrichment of the current environment based on `IHostEnvironment`
- Conversion of log levels hence you can continue to use the `Logging` section.
- [SEQ](https://datalust.co/seq) is added by default when `Logging:Seq:ServerUrl` or `Seq:ServerUrl` configuration is available. (Use `Logging:Seq:ApiKey` or `Seq:ApiKey` to set the key).
- Registration via `IServiceCollection` and `IHostBuilder` (with the defaults above)

Consult the [sample](https://github.com/tinglesoftware/dotnet-extensions/tree/main/samples/SerilogSample) or the [builder](https://github.com/tinglesoftware/dotnet-extensions/blob/main/src/Tingle.Extensions.Serilog/SerilogBuilder.cs) for more on how to use or what happens underneath.
