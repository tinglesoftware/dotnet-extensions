namespace Tingle.Extensions.Mustache.Tests;

public class MustacheTemplateTests
{
    [Fact]
    public void SimpleReplacementWorks()
    {
        var template = new MustacheTemplate("Your OTP code is {{otp}} expires in {{minutes}} minutes.");
        var values = new Dictionary<string, object?>
        {
            ["otp"] = "12345",
            ["minutes"] = 59,
        };
        var rendered = template.Render(values);
        Assert.Equal("Your OTP code is 12345 expires in 59 minutes.", rendered);
    }

    [Fact]
    public void SimpleReplacementWorks_With_Brackets()
    {
        var template = new MustacheTemplate("{{name}} ({{borrowerPhone}}) has paid {{repayment}} to {{id}}. New balance is {{balance}}");
        var values = new Dictionary<string, object?>
        {
            ["name"] = "GILBERT ONYIEGO OMBONGI",
            ["borrowerPhone"] = "070703****",
            ["repayment"] = "KES 100.00",
            ["id"] = "C55067286",
            ["balance"] = "KES 2,500.00",
        };
        var rendered = template.Render(values);
        Assert.Equal("GILBERT ONYIEGO OMBONGI (070703****) has paid KES 100.00 to C55067286. New balance is KES 2,500.00", rendered);
    }

    [Fact]
    public void CaseInsensitiveSimpleReplacementWorks()
    {
        var template = new MustacheTemplate("Your OTP code is {{otp}} expires in {{ minutes }} minutes.", true);
        var values = new Dictionary<string, object?>
        {
            ["otp"] = "12345",
            ["Minutes"] = 59,
        };
        var rendered = template.Render(values);
        Assert.Equal("Your OTP code is 12345 expires in 59 minutes.", rendered);
    }

    [Fact]
    public void NestedVaribleInterpolationWorks()
    {
        var template = new MustacheTemplate("Welcome home {{ user.name }}");
        var values = new Dictionary<string, object?>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John",
            },
        };
        var rendered = template.Render(values);
        Assert.Equal("Welcome home John", rendered);
    }

    [Theory]
    [InlineData("{{#user}}\r\nWelcome home {{ name }}\r\n{{/user}}", "\r\nWelcome home John\r\n")]
    [InlineData("{{#user}}Welcome home {{ name }}{{/user}}", "Welcome home John")]
    public void ScopingWorks(string src, string expected)
    {
        var template = new MustacheTemplate(src);
        var values = new Dictionary<string, object?>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John",
            },
        };
        var actual = template.Render(values);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("{{ name }} vehicles:{{#each vehicles}}\r\n{{ . }}{{/each}}", "John Kamau vehicles:\r\n123 KL 3\r\n054F 87T")]
    [InlineData("{{ name }} vehicles:{{#each vehicles}}{{ . }}{{/each}}", "John Kamau vehicles:123 KL 3054F 87T")]
    public void SimpleCollectionHandlingWorks(string src, string expected)
    {
        var template = new MustacheTemplate(src);
        var values = new Dictionary<string, object?>
        {
            ["name"] = "John Kamau",
            ["vehicles"] = new List<string>
                {
                    "123 KL 3",
                    "054F 87T",
                },
        };
        var actual = template.Render(values);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("{{ name }} vehicles:{{#each vehicles}}\r\n{{ registration }}{{/each}}", "John Kamau vehicles:\r\n123 KL 3\r\n054F 87T")]
    [InlineData("{{ name }} vehicles:{{#each vehicles}}{{ registration }}{{/each}}", "John Kamau vehicles:123 KL 3054F 87T")]
    public void ScalarCollectionHandlingWorks(string src, string expected)
    {
        var template = new MustacheTemplate(src);
        var values = new Dictionary<string, object?>
        {
            ["name"] = "John Kamau",
            ["vehicles"] = new List<Dictionary<string, object>>
            {
                new() { ["registration"] = "123 KL 3", },
                new() { ["registration"] = "054F 87T", },
            },
        };
        var actual = template.Render(values);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AdvancedInterpolationWorks()
    {
        var template = new MustacheTemplate("{{#each vehicles}}{{ registration }} is owned by: {{../../name}}\r\n{{/each}}");
        var values = new Dictionary<string, object?>
        {
            ["name"] = "John Kamau",
            ["vehicles"] = new List<Dictionary<string, object>>
            {
                new() { ["registration"] = "123 KL 3", },
                new() { ["registration"] = "054F 87T", },
            },
        };
        var expected = "123 KL 3 is owned by: John Kamau\r\n054F 87T is owned by: John Kamau\r\n";
        var actual = template.Render(values);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AdvancedValueHandlingWorks()
    {
        var src = "{{#each vehicles}}{{registration}} is owned by: {{../../name}}{{#country}} in {{.}}{{/country}}\r\n{{/each}}";
        var template = new MustacheTemplate(src);
        var values = new Dictionary<string, object?>
        {
            ["name"] = "John Kamau",
            ["vehicles"] = new List<Dictionary<string, object>>
            {
                new() {
                    ["registration"] = "123 KL 3",
                    ["country"] = "Kenya",
                },
                new() { ["registration"] = "054F 87T", },
            },
        };
        var expected = "123 KL 3 is owned by: John Kamau in Kenya\r\n054F 87T is owned by: John Kamau\r\n";
        var actual = template.Render(values);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InvertedGroupsWorks()
    {
        var src = "{{#each vehicles}}{{registration}} is owned by: {{../../name}}{{#country}} in {{.}}{{/country}}{{^registered}} (unknown date){{/registered}}\r\n{{/each}}";
        var template = new MustacheTemplate(src);
        var values = new Dictionary<string, object?>
        {
            ["name"] = "John Kamau",
            ["vehicles"] = new List<Dictionary<string, object>>
                {
                    new() {
                        ["registration"] = "123 KL 3",
                        ["country"] = "Kenya",
                        ["registered"] = "16 June 2020",
                    },
                    new() {
                        ["registration"] = "054F 87T",
                        ["country"] = "Kenya",
                    },
                },
        };
        var expected = "123 KL 3 is owned by: John Kamau in Kenya\r\n054F 87T is owned by: John Kamau in Kenya (unknown date)\r\n";
        var actual = template.Render(values);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InferredSimpleReplacementWorks()
    {
        var template = new MustacheTemplate("Your OTP code is {{otp}} expires in {{minutes}} minutes.", inference: true);
        var values = new Dictionary<string, object?>
        {
            ["otp"] = "12345",
        };
        var rendered = template.RenderInferred(values);
        Assert.Equal("Your OTP code is 12345 expires in minutes_Value minutes.", rendered);
    }

    [Fact]
    public void InferredNestedVaribleInterpolationWorks()
    {
        var template = new MustacheTemplate("Welcome home {{ user.name }}", inference: true);
        var values = new Dictionary<string, object?>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["names"] = "John",
            },
        };
        var rendered = template.RenderInferred(values);
        Assert.Equal("Welcome home name_Value", rendered);
    }

    [Fact]
    public void WrongProvidedValueIsIgnoredInFavoutOfInferredValue()
    {
        var template = new MustacheTemplate("Welcome home {{ user.name }}", inference: true);
        var values = new Dictionary<string, object?>
        {
            ["user"] = "John", // should have been a dictionary
        };
        var rendered = template.RenderInferred(values);
        Assert.Equal("Welcome home name_Value", rendered);
    }
}
