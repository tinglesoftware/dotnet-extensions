using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Tingle.Extensions.Logging.Tests;

public class CliConsoleFormatterTests
{
    [Fact]
    public void Works()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddCliConsole());
        var provider = services.BuildServiceProvider();

        // ensure a formatter is registered
        var formatters = provider.GetServices<ConsoleFormatter>();
        var formatter = Assert.Single(formatters.OfType<CliConsoleFormatter>());

        // ensure the formatter name is set
        var loggerOptions = provider.GetRequiredService<IOptions<ConsoleLoggerOptions>>();
        Assert.Equal("cli", loggerOptions.Value.FormatterName);

        // TODO: complete this test with more logic
    }
}
