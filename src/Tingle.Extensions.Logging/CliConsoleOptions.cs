using Microsoft.Extensions.Logging.Console;

namespace Tingle.Extensions.Logging;

/// <summary>
/// Options for the CLI console log formatter.
/// </summary>
public class CliConsoleOptions : SimpleConsoleFormatterOptions
{
    /// <summary>Includes category when true.</summary>
    public bool IncludeCategory { get; set; }

    /// <summary>Includes event id when true.</summary>
    public bool IncludeEventId { get; set; }
}
