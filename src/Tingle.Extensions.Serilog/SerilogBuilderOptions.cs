using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Exceptions.Core;

namespace Microsoft.Extensions.DependencyInjection;

internal class SerilogBuilderOptions
{
    public List<Action<DestructuringOptionsBuilder>> ConfigureDestructuringOptions { get; } = [];
    public List<Action<SensitiveDataEnricherOptions>> ConfigureSensitiveDataEnricherOptions { get; } = [];
    public List<Action<LoggerConfiguration>> ConfigureLoggerConfiguration { get; } = [];
}
