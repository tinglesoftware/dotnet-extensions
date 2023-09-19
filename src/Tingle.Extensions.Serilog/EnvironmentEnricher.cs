using Microsoft.Extensions.Hosting;
using Serilog.Core;
using Serilog.Events;
using System.Reflection;

namespace Tingle.Extensions.Serilog;

internal class EnvironmentEnricher : ILogEventEnricher
{
    private readonly IHostEnvironment environment;

    private LogEventProperty? applicationName, applicationVersion;
    private LogEventProperty? environmentName, machineName;

    public EnvironmentEnricher(IHostEnvironment environment)
    {
        this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        applicationName ??= propertyFactory.CreateProperty(
            name: "ApplicationName",
            value: environment.ApplicationName);

        applicationVersion ??= propertyFactory.CreateProperty(
            name: "ApplicationVersion",
            value: GetVersion());

        environmentName ??= propertyFactory.CreateProperty(
            name: "EnvironmentName",
            value: environment.EnvironmentName);

        machineName ??= propertyFactory.CreateProperty(
            name: "MachineName",
            value: Environment.MachineName);

        logEvent.AddPropertyIfAbsent(applicationName);
        logEvent.AddPropertyIfAbsent(applicationVersion);
        logEvent.AddPropertyIfAbsent(environmentName);
        logEvent.AddPropertyIfAbsent(machineName);
    }

    private static string GetVersion()
    {
        var assembly = Assembly.GetEntryAssembly()!;
        var attr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        return attr != null && !string.IsNullOrWhiteSpace(attr.InformationalVersion)
            ? attr.InformationalVersion
            : assembly.GetName().Version!.ToString(3);
    }
}