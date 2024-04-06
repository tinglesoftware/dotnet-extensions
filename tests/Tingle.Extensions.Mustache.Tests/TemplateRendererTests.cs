using System.Text.Json;
using Tingle.Extensions.Mustache.Parsing;
using Tingle.Extensions.Mustache.Rendering;

namespace Tingle.Extensions.Mustache.Tests;

public class TemplateRendererTests
{
    [Fact]
    public void WorksWithDictionary()
    {
        var src = "Hello {{Name}}! Please come to {{office_number}}.";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);

        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { });
        var values = new Dictionary<string, object?>
        {
            ["Name"] = "Kimndapu1991",
            ["office_number"] = "E1",
        };
        var rendered = renderer.Render(values);
        Assert.Equal("Hello Kimndapu1991! Please come to E1.", rendered);
    }

    [Fact]
    public void WorksWithDictionaryCaseInsensitive()
    {
        var src = "Hello {{Name}}! Please come to {{office_number}}.";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);

        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { IgnoreCase = true, });
        var values = new Dictionary<string, object?>
        {
            ["nAME"] = "Kimndapu1991",
            ["OFFICE_NUMBER"] = "E1",
        };
        var rendered = renderer.Render(values);
        Assert.Equal("Hello Kimndapu1991! Please come to E1.", rendered);
    }

    [Fact]
    public void WorksWithAnonymousType()
    {
        var src = "Hello {{Name}}! Please come to {{office_number}}.";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);

        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { });
        var values = new
        {
            Name = "Kimndapu1991",
            office_number = "E1",
        };
        var rendered = renderer.Render(values);
        Assert.Equal("Hello Kimndapu1991! Please come to E1.", rendered);
    }

    [Fact]
    public void WorksWithAnonymousTypeCaseInsensitive()
    {
        var src = "Hello {{nAME}}! Please come to {{OFFICE_NUMBER}}.";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);

        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { IgnoreCase = true });
        var values = new
        {
            Name = "Kimndapu1991",
            office_number = "E1",
        };
        var rendered = renderer.Render(values);
        Assert.Equal("Hello Kimndapu1991! Please come to E1.", rendered);
    }

    [Fact]
    public void WorksWithGrouping()
    {
        var src = "Hello! {{#user}}Your name is {{name}}.{{/user}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);

        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { });
        var values = new Dictionary<string, object?>
        {
            ["user"] = new Dictionary<string, object?>
            {
                ["name"] = "KimNdapu",
            },
        };
        var rendered = renderer.Render(values);
        Assert.Equal("Hello! Your name is KimNdapu.", rendered);
    }

    [Theory]
    [InlineData(null, "Hello! What is your name.")]
    [InlineData("KimNdapu", "Hello!")]
    public void WorksWithInvertingGrouping(string? name, string expected)
    {
        var src = "Hello!{{^name}} What is your name.{{/name}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);

        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { });
        var values = new Dictionary<string, object?>
        {
            ["name"] = name,
        };
        var actual = renderer.Render(values);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void WorksWithSimpleCollections()
    {
        var src = "Hello!{{#each cars}}\r\nRegistration: {{.}}{{/each}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);

        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { });
        var values = new Dictionary<string, object?>
        {
            ["cars"] = new List<string>
                {
                    "123 KL 3",
                    "054F 87T",
                },
        };
        var rendered = renderer.Render(values);
        Assert.Equal("Hello!\r\nRegistration: 123 KL 3\r\nRegistration: 054F 87T", rendered);
    }

    [Fact]
    public void WorksWithScalarCollections()
    {
        var src = "Hello!{{#each cars}}\r\nRegistration: {{registration}}{{/each}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);

        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { });
        var values = new Dictionary<string, object?>
        {
            ["cars"] = new List<Dictionary<string, object>>
            {
                new() { ["registration"] = "123 KL 3", },
                new() { ["registration"] = "054F 87T", },
            },
        };
        var rendered = renderer.Render(values);
        Assert.Equal("Hello!\r\nRegistration: 123 KL 3\r\nRegistration: 054F 87T", rendered);
    }

    [Fact]
    public void AdvancedInterpolationWorks()
    {
        var src = "{{#each vehicles}}{{ registration }} is owned by: {{../../name}}\r\n{{/each}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);

        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { });
        var values = new Dictionary<string, object?>
        {
            ["name"] = "John Kamau",
            ["vehicles"] = new List<Dictionary<string, object>>
            {
                new() { ["registration"] = "123 KL 3", },
                new() { ["registration"] = "054F 87T", },
            },
        };
        var actual = renderer.Render(values);
        Assert.Equal("123 KL 3 is owned by: John Kamau\r\n054F 87T is owned by: John Kamau\r\n", actual);
    }

    [Fact]
    public void CanInferCollection()
    {
        var src = "{{#Person}}{{Name}}{{#each ../Person.FavoriteColors}}{{.}}{{/each}}{{/Person}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);
        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { }, inference: true);

        var expected = @"{
                        ""Person"" :{
                            ""Name"" : ""Name_Value"",
                            ""FavoriteColors"" : [
                                ""FavoriteColors_1"",
                                ""FavoriteColors_2"",
                                ""FavoriteColors_3""
                             ]
                        }
                    }".EliminateWhitespace();

        var actual = SerializeInferredModel(renderer.GetInferredModel()).EliminateWhitespace();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanInferScalar()
    {
        var src = "{{Name}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);
        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { }, inference: true);

        var expected = @"{""Name"" : ""Name_Value""}".EliminateWhitespace();
        var actual = SerializeInferredModel(renderer.GetInferredModel()).EliminateWhitespace();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanInferNestedProperties()
    {
        var src = "{{#Person}}{{Name}}{{/Person}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);
        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { }, inference: true);

        var expected = @"{
                        ""Person"" :{
                            ""Name"" : ""Name_Value""
                        }
                    }".EliminateWhitespace();

        var actual = SerializeInferredModel(renderer.GetInferredModel()).EliminateWhitespace();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ProducesEmptyObjectWhenTemplateHasNoMustacheMarkup()
    {
        var src = "This template has no mustache things.";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);
        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { }, inference: true);

        var expected = @"{}".EliminateWhitespace();
        var actual = SerializeInferredModel(renderer.GetInferredModel()).EliminateWhitespace();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RendersCollectionObjectsWhenUsed()
    {
        var src = "{{#each Employees}}{{name}}{{/each}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);
        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { }, inference: true);

        var expected = @"{""Employees"" : [{ ""name"" : ""name_Value""}]}".EliminateWhitespace();
        var actual = SerializeInferredModel(renderer.GetInferredModel()).EliminateWhitespace();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RendersCollectionSubObjectsWhenUsed()
    {
        var src = "{{#each Employees}}{{person.name}}{{#each favoriteColors}}{{hue}}{{/each}}{{#each workplaces}}{{.}}{{/each}}{{/each}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);
        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { }, inference: true);

        var expected = @"{
                        ""Employees"" : [{ 
                            ""person"" : { ""name"" : ""name_Value""},
                            ""favoriteColors"" : [{""hue"" : ""hue_Value""}],                                
                            ""workplaces"" : [ ""workplaces_1"",""workplaces_2"",""workplaces_3"" ] 
                            }]
                        }".EliminateWhitespace();

        var actual = SerializeInferredModel(renderer.GetInferredModel()).EliminateWhitespace();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("{{Mike", "{{{{name}}")]
    [InlineData("{Mike", "{{{name}}")]
    [InlineData("Mike}", "{{name}}}")]
    [InlineData("Mike}}", "{{name}}}}")]
    public void HandlesPartialOpenAndPartialClose(string expected, string src)
    {
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);
        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { });

        var model = new Dictionary<string, object?>
        {
            ["name"] = "Mike"
        };

        var actual = renderer.Render(model);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("<wbr>", "{{content}}", "&lt;wbr&gt;")]
    [InlineData("<wbr>", "{{{content}}}", "<wbr>")]
    public void ValueEscapingIsActivatedBasedOnValueInterpolationMustacheSyntax(string content, string src, string expected)
    {
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);
        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { });

        var model = new Dictionary<string, object?>
        {
            ["content"] = content,
        };
        var actual = renderer.Render(model);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("<wbr>", "{{content}}", "<wbr>")]
    [InlineData("<wbr>", "{{{content}}}", "<wbr>")]
    public void ValueEscapingIsDisabledWhenRequested(string content, string src, string expected)
    {
        var parser = new TemplateParser(new TemplateParserOptions { });
        var tokens = parser.Parse(src);
        var renderer = new TemplateRenderer(tokens, new TemplateRenderingOptions { DisableContentSafety = true, });

        var model = new Dictionary<string, object?>
        {
            ["content"] = content,
        };
        var actual = renderer.Render(model);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void HtmlIsNotEscapedWhenUsingUnsafeSyntaxes()
    {
        var src = @"{{{stuff}}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var renderer = new TemplateRenderer(parser.Parse(src), new TemplateRenderingOptions { });

        var model = new Dictionary<string, object?>
        {
            ["stuff"] = "<b>inner</b>"
        };

        var actual = renderer.Render(model);
        Assert.Equal("<b>inner</b>", actual);

        src = @"{{&stuff}}";
        renderer = new TemplateRenderer(parser.Parse(src), new TemplateRenderingOptions { });
        actual = renderer.Render(model);
        Assert.Equal("<b>inner</b>", actual);
    }

    [Fact]
    public void HtmlIsEscapedByDefault()
    {
        var model = new Dictionary<string, object?>
        {
            ["stuff"] = "<b>inner</b>"
        };

        var src = @"{{stuff}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var renderer = new TemplateRenderer(parser.Parse(src), new TemplateRenderingOptions { });
        var actual = renderer.Render(model);

        Assert.Equal("&lt;b&gt;inner&lt;/b&gt;", actual);
    }

    [Fact]
    public void CommentsAreExcludedFromOutput()
    {
        var model = new Dictionary<string, object?>();

        var src = @"as{{!stu
                    ff}}df";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var renderer = new TemplateRenderer(parser.Parse(src), new TemplateRenderingOptions { });
        var actual = renderer.Render(model);
        Assert.Equal("asdf", actual);
    }

    [Fact]
    public void NegationGroupRendersContentWhenValueNotSet()
    {
        var model = new Dictionary<string, object?>();

        var src = @"{{^stuff}}No Stuff Here.{{/stuff}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var renderer = new TemplateRenderer(parser.Parse(src), new TemplateRenderingOptions { });
        var actual = renderer.Render(model);
        Assert.Equal("No Stuff Here.", actual);
    }

    [Fact]
    public void UnsignedIntegralTypeModelVariablesAreSupported()
    {
        var model = new Dictionary<string, object?>(){
                        {"uint", (uint)123},
                        {"ushort", (ushort)234},
                        {"ulong", 18446744073709551615} // max ulong
                    };

        var src = "{{uint}};{{ushort}};{{ulong}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var renderer = new TemplateRenderer(parser.Parse(src), new TemplateRenderingOptions { });
        var actual = renderer.Render(model);
        Assert.Equal("123;234;18446744073709551615", actual);
    }

    [Fact]
    public void TemplateRendersWithComplextEachPath()
    {
        var src = @"{{#each Company.ceo.products}}<li>{{ name }} and {{version}} and has a CEO: {{../../last_name}}</li>{{/each}}";
        var parser = new TemplateParser(new TemplateParserOptions { });
        var renderer = new TemplateRenderer(parser.Parse(src), new TemplateRenderingOptions { });

        var model = new Dictionary<string, object?>();

        var company = new Dictionary<string, object?>();
        model["Company"] = company;

        var ceo = new Dictionary<string, object?>();
        company["ceo"] = ceo;
        ceo["last_name"] = "Smith";

        var products = Enumerable.Range(0, 3).Select(k => new Dictionary<string, object?>
        {
            ["name"] = "name " + k,
            ["version"] = "version " + k
        }).ToArray();

        ceo["products"] = products;

        var actual = renderer.Render(model);

        Assert.Equal("<li>name 0 and version 0 and has a CEO: Smith</li>" +
            "<li>name 1 and version 1 and has a CEO: Smith</li>" +
            "<li>name 2 and version 2 and has a CEO: Smith</li>", actual);
    }

    private static string SerializeInferredModel(object model) => JsonSerializer.Serialize(model);
}
