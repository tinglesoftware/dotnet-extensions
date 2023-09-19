using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Configuration;

namespace Tingle.Extensions.Serilog;

internal class ConvertedSerilogSettings : ILoggerSettings
{
    private readonly IConfiguration configuration;

    public ConvertedSerilogSettings(IConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void Configure(LoggerConfiguration loggerConfiguration)
    {
        const string DefaultKey = "Default";

        // configure provider-agnostic levels
        var children = configuration.GetSection("LogLevel").GetChildren();
        foreach (var child in children)
        {
            var source = child.Key;
            var level = Enum.Parse<LogLevel>(child.Value!).ToLogEventLevel();
            if (string.Equals(source, DefaultKey, StringComparison.OrdinalIgnoreCase))
            {
                loggerConfiguration.MinimumLevel.Is(level);
            }
            else
            {
                loggerConfiguration.MinimumLevel.Override(source, level);
            }
        }
    }
}
