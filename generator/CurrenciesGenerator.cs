using System.CodeDom.Compiler;
using System.Text.Json.Serialization;

namespace StaticDataGenerator;

internal class CurrenciesGenerator(IHostEnvironment environment, FileDownloader downloader)
    : AbstractGenerator<Dictionary<string, CurrenciesGenerator.CurrencyImpl>>(environment, downloader, "iso-4217.json")
{
    internal struct CurrencyImpl
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("symbolNative")]
        public string SymbolNative { get; set; }

        [JsonPropertyName("decimalDigits")]
        public int? DecimalDigits { get; set; }

        [JsonPropertyName("namePlural")]
        public string NamePlural { get; set; }
    }

    protected override void WriteCode(IndentedTextWriter writer, Dictionary<string, CurrencyImpl> data)
    {
        // remove entries without all the information
        foreach (var kvp in data.ToList())
        {
            var impl = kvp.Value;
            if (impl.Symbol is null
                || impl.Code is null
                || impl.Name is null
                || impl.SymbolNative is null
                || impl.DecimalDigits is null
                || impl.NamePlural is null)
            {
                data.Remove(kvp.Key);
            }
        }

        writer.WriteLine("// The currencies");
        foreach (var kvp in data)
        {
            var impl = kvp.Value;
            writer.Write($"public static readonly Currency {impl.Code}");
            writer.WriteLine($" = new(\"{impl.Code}\", \"{impl.Symbol}\", \"{impl.SymbolNative}\", \"{impl.Name}\", \"{impl.NamePlural}\", \"{impl.DecimalDigits}\");");
        }
        writer.Indent--;

        writer.WriteLine();
        writer.Indent++;
        writer.WriteLine("// The array/list");
        writer.WriteLine("internal static readonly IReadOnlyList<Currency> All = new List<Currency>");
        writer.WriteLine("{");
        writer.Indent++;
        foreach (var kvp in data)
        {
            var impl = kvp.Value;
            writer.WriteLine($"{ClassName}.{impl.Code},");
        }
        writer.Indent--;
        writer.WriteLine("};");
    }

    protected override void WriteJson(IndentedTextWriter writer, Dictionary<string, CurrencyImpl> data)
    {
        var impls = data.Values.ToList();
        writer.WriteLine("[");
        writer.Indent++;
        foreach (var impl in impls)
        {
            writer.Write($"{{ \"name\": \"{impl.Name}\", \"code\": \"{impl.Code}\"}}");
            writer.WriteLine(impls.IndexOf(impl) == data.Count - 1 ? "" : ",");
        }
        writer.Indent--;
        writer.WriteLine("]");
    }
}
