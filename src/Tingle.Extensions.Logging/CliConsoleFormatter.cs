using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace Tingle.Extensions.Logging;

// We have this because the SimpleConsoleFormatter does not offset follow up lines with the size of the time, log level, and its padding
// and does not support excluding category and eventId
internal class CliConsoleFormatter : ConsoleFormatter
{
    private const string LogLevelPadding = ": ";
    private readonly IDisposable? _optionsReloadToken;

    public CliConsoleFormatter(IOptionsMonitor<CliConsoleOptions> options) : base("cli")
    {
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    private void ReloadLoggerOptions(CliConsoleOptions options)
    {
        FormatterOptions = options;
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }

    internal CliConsoleOptions? FormatterOptions { get; set; }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }
        LogLevel logLevel = logEntry.LogLevel;
        ConsoleColors logLevelColors = GetLogLevelConsoleColors(logLevel);
        string logLevelString = GetLogLevelString(logLevel);

        var timestamp = (string?)null;
        var timestampFormat = FormatterOptions?.TimestampFormat;
        if (timestampFormat != null)
        {
            DateTimeOffset dateTimeOffset = GetCurrentDateTime();
            timestamp = dateTimeOffset.ToString(timestampFormat);
        }
        if (timestamp != null)
        {
            textWriter.Write(timestamp);
        }
        if (logLevelString != null)
        {
            textWriter.WriteColoredMessage(logLevelString, logLevelColors.Background, logLevelColors.Foreground);
        }
        CreateDefaultLogMessage(textWriter, logEntry, message, scopeProvider);
    }

    private void CreateDefaultLogMessage<TState>(TextWriter textWriter, in LogEntry<TState> logEntry, string message, IExternalScopeProvider? scopeProvider)
    {
        bool singleLine = FormatterOptions?.SingleLine ?? false;
        bool includeCategory = FormatterOptions?.IncludeCategory ?? false;
        bool includeEventId = FormatterOptions?.IncludeEventId ?? false;
        int eventId = logEntry.EventId.Id;
        var exception = logEntry.Exception;

        // Example:
        // info: ConsoleApp.Program[10]
        //       Request received

        // category and event id
        textWriter.Write(LogLevelPadding);
        if (includeCategory)
        {
            textWriter.Write(logEntry.Category);

            if (includeEventId)
            {
                textWriter.Write('[');

#if NETCOREAPP
                Span<char> span = stackalloc char[10];
                if (eventId.TryFormat(span, out int charsWritten))
                    textWriter.Write(span[..charsWritten]);
                else
#endif
                textWriter.Write(eventId.ToString());

                textWriter.Write(']');
            }
            if (!singleLine)
            {
                textWriter.Write(Environment.NewLine);
            }
        }

        var paddings = MessagePaddings.Create(FormatterOptions);
        // scope information
        WriteScopeInformation(textWriter, scopeProvider, singleLine, paddings);
        WriteMessage(textWriter, message, singleLine, paddings);

        // Example:
        // System.InvalidOperationException
        //    at Namespace.Class.Function() in File:line X
        if (exception != null)
        {
            // exception message
            WriteMessage(textWriter, exception.ToString(), singleLine, paddings);
        }
        if (singleLine)
        {
            textWriter.Write(Environment.NewLine);
        }
    }

    private static void WriteMessage(TextWriter textWriter, string message, bool singleLine, MessagePaddings paddings)
    {
        if (!string.IsNullOrEmpty(message))
        {
            if (singleLine)
            {
                textWriter.Write(' ');
                WriteReplacing(textWriter, Environment.NewLine, " ", message);
            }
            else
            {
                textWriter.Write(paddings.MessagePadding);
                WriteReplacing(textWriter, Environment.NewLine, paddings.NewLineWithMessagePadding, message);
                textWriter.Write(Environment.NewLine);
            }
        }

        static void WriteReplacing(TextWriter writer, string oldValue, string newValue, string message)
        {
            string newMessage = message.Replace(oldValue, newValue);
            writer.Write(newMessage);
        }
    }

    private DateTimeOffset GetCurrentDateTime()
    {
        return FormatterOptions?.UseUtcTimestamp ?? false ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
    {
        bool disableColors = FormatterOptions?.ColorBehavior == LoggerColorBehavior.Disabled ||
            FormatterOptions?.ColorBehavior == LoggerColorBehavior.Default && Console.IsOutputRedirected;
        if (disableColors)
        {
            return new ConsoleColors(null, null);
        }
        // We must explicitly set the background color if we are setting the foreground color,
        // since just setting one can look bad on the users console.
        return logLevel switch
        {
            LogLevel.Trace => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            LogLevel.Debug => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            LogLevel.Information => new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black),
            LogLevel.Warning => new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black),
            LogLevel.Error => new ConsoleColors(ConsoleColor.Black, ConsoleColor.DarkRed),
            LogLevel.Critical => new ConsoleColors(ConsoleColor.White, ConsoleColor.DarkRed),
            _ => new ConsoleColors(null, null)
        };
    }

    private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider? scopeProvider, bool singleLine, MessagePaddings paddings)
    {
        if (FormatterOptions?.IncludeScopes ?? false && scopeProvider != null)
        {
            bool paddingNeeded = !singleLine;
            scopeProvider?.ForEachScope((scope, state) =>
            {
                if (paddingNeeded)
                {
                    paddingNeeded = false;
                    state.Write(paddings.MessagePadding);
                    state.Write("=> ");
                }
                else
                {
                    state.Write(" => ");
                }
                state.Write(scope);
            }, textWriter);

            if (!paddingNeeded && !singleLine)
            {
                textWriter.Write(Environment.NewLine);
            }
        }
    }

    private readonly struct ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
    {
        public ConsoleColor? Foreground { get; } = foreground;

        public ConsoleColor? Background { get; } = background;
    }

    private readonly struct MessagePaddings(string messagePadding, string newLineWithMessagePadding)
    {
        public string MessagePadding { get; } = messagePadding;
        public string NewLineWithMessagePadding { get; } = newLineWithMessagePadding;

        public static MessagePaddings Create(CliConsoleOptions? options)
        {
            string messagePadding, newLineWithMessagePadding;
            var timestampFormat = options?.TimestampFormat;
            if (timestampFormat is not null)
            {
                messagePadding = new(' ', GetLogLevelString(LogLevel.Information).Length + LogLevelPadding.Length + timestampFormat.Length);
            }
            else
            {
                messagePadding = new(' ', GetLogLevelString(LogLevel.Information).Length + LogLevelPadding.Length);
            }

            newLineWithMessagePadding = Environment.NewLine + messagePadding;

            return new(messagePadding, newLineWithMessagePadding);
        }
    }
}
