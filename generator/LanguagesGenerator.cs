using System.CodeDom.Compiler;
using System.Text.RegularExpressions;

namespace StaticDataGenerator;

internal partial class LanguagesGenerator(IHostEnvironment environment, FileDownloader downloader) 
    : AbstractGenerator<List<LanguagesGenerator.LanguageImpl>>(environment, downloader, "iso-639-3.tab")
{
    internal record struct LanguageImpl(string Name, string Part3, string? Part1, string Type, string Scope);

    protected override ValueTask<List<LanguageImpl>?> ParseAsync(Stream dataStream, CancellationToken cancellationToken = default)
    {
        static string? NullifyIfNecessary(string input) => string.IsNullOrWhiteSpace(input) ? null : input;

        var regex = LineFormat();
        var records = new List<LanguageImpl>();
        var reader = new StreamReader(dataStream);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var match = regex.Match(line);
            if (!match.Success)
            {
                if (records.Count > 0) throw new InvalidDataException($"\"{line}\" does not match the RegEx");
                continue;
            }

            var name = match.Groups["name"].Value;
            var part3 = match.Groups["p3"].Value;
            var part1 = NullifyIfNecessary(match.Groups["p1"].Value);
            var type = GetLanguageType(match.Groups["type"].Value[0]);
            var scope = GetLanguageScope(match.Groups["scope"].Value[0]);

            // remove records without necessary info data
            if (name is null || part3 is null || scope is null || type is null) continue;
            records.Add(new LanguageImpl(Name: name, Part3: part3, Part1: part1, Type: type, Scope: scope));
        }

        return ValueTask.FromResult<List<LanguageImpl>?>(records);
    }

    protected override void WriteCode(IndentedTextWriter writer, List<LanguageImpl> data)
    {
        writer.WriteLine("// The languages");
        foreach (var impl in data)
        {
            writer.Write($"public static readonly Language Language_{impl.Part3}");
            var twoLetterCode = impl.Part1 is null ? "null" : $"\"{impl.Part1}\"";
            writer.WriteLine($" = new(\"{impl.Name}\", \"{impl.Part3}\", {twoLetterCode}, Language.{impl.Type}, Language.{impl.Scope});");
        }
        writer.Indent--;

        writer.WriteLine();
        writer.Indent++;
        writer.WriteLine("// The array/list");
        writer.WriteLine("internal static readonly IReadOnlyList<Language> All = new List<Language>");
        writer.WriteLine("{");
        writer.Indent++;
        foreach (var impl in data)
        {
            writer.WriteLine($"{ClassName}.Language_{impl.Part3},");
        }
        writer.Indent--;
        writer.WriteLine("};");
    }

    protected override void WriteJson(IndentedTextWriter writer, List<LanguageImpl> data)
    {
        writer.WriteLine("[");
        writer.Indent++;
        foreach (var impl in data)
        {
            writer.Write($"{{ \"name\": \"{impl.Name}\", \"code\": \"{impl.Part3}\"}}");
            writer.WriteLine(data.IndexOf(impl) == data.Count - 1 ? "" : ",");
        }
        writer.Indent--;
        writer.WriteLine("]");
    }

    private static string GetLanguageScope(char chr)
    {
        return chr switch
        {
            'C' => "LanguageScope.Collective",
            'I' => "LanguageScope.Individual",
            'L' => "LanguageScope.Local",
            'M' => "LanguageScope.MacroLanguage",
            'S' => "LanguageScope.Special",
            _ => throw new ArgumentException("Invalid character.", nameof(chr)),
        };
    }
    private static string GetLanguageType(char chr)
    {
        return chr switch
        {
            'L' => "LanguageType.Living",
            'E' => "LanguageType.Extinct",
            'C' => "LanguageType.Constructed",
            'A' => "LanguageType.Ancient",
            'H' => "LanguageType.Historical",
            'S' => "LanguageType.Special",
            _ => throw new ArgumentException("Invalid character.", nameof(chr)),
        };
    }

    // inspiration from https://github.com/ForeverZer0/Iso639
    [GeneratedRegex("(?<p3>[a-z]{3})\\t(?<p2b>[a-z]{3})?\\t(?<p2t>[a-z]{3})?\\t(?<p1>[a-z]{2})?\\t(?<scope>[A-Z])\\t(?<type>[A-Z])\\t(?<name>.+)\\t(?<comment>.*)")]
    private static partial Regex LineFormat();
}
