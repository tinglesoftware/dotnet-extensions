using System.CodeDom.Compiler;
using System.Text.Json.Serialization;

namespace StaticDataGenerator;

internal class CountriesGenerator(IHostEnvironment environment, FileDownloader downloader)
    : AbstractGenerator<List<CountriesGenerator.CountryImpl>>(environment, downloader, "countries-20200124.json")
{
    internal struct CountryImpl
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("name_safe")]
        public string NameSafe { get; set; }

        [JsonPropertyName("continent")]
        public string Continent { get; set; }

        [JsonPropertyName("code_numeric")]
        public string CodeNumeric { get; set; }

        [JsonPropertyName("code_2")]
        public string Code2 { get; set; }

        [JsonPropertyName("code_3")]
        public string Code3 { get; set; }

        [JsonPropertyName("flag_url")]
        public string FlagUrl { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }
    }

    protected override void WriteCode(IndentedTextWriter writer, List<CountryImpl> data)
    {
        // remove entries without all the information
        data.RemoveAll(impl =>
        {
            return impl.Name is null
                || impl.Continent is null
                || impl.CodeNumeric is null
                || impl.Code2 is null
                || impl.Code3 is null
                || impl.FlagUrl is null
                || impl.Currency is null;
        });


        writer.WriteLine("// The currencies");
        foreach (var impl in data)
        {
            writer.Write($"public static readonly Country {impl.NameSafe}");
            writer.WriteLine($" = new(\"{impl.Name}\", \"{impl.Continent}\", \"{impl.CodeNumeric}\", \"{impl.Code2}\", \"{impl.Code3}\", \"{impl.FlagUrl}\", \"{impl.Currency}\");");
        }
        writer.Indent--;

        // numeric code mapping
        writer.WriteLine();
        writer.WriteLine("// The array/list");
        writer.WriteLine("internal static readonly IReadOnlyList<Country> All = new List<Country>");
        writer.WriteLine("{");
        writer.Indent++;
        foreach (var impl in data)
        {
            writer.WriteLine($"{ClassName}.{impl.NameSafe},");
        }
        writer.Indent--;
        writer.WriteLine("};");
        writer.Indent--;
    }

    protected override void WriteJson(IndentedTextWriter writer, List<CountryImpl> data)
    {
        writer.WriteLine("[");
        writer.Indent++;
        foreach (var impl in data)
        {
            writer.Write($"{{ \"name\": \"{impl.Name}\", \"codeAlpha2\": \"{impl.Code2}\", \"codeAlpha3\": \"{impl.Code3}\"}}");
            writer.WriteLine(data.IndexOf(impl) == data.Count - 1 ? "" : ",");
        }
        writer.Indent--;
        writer.WriteLine("]");
    }
}
