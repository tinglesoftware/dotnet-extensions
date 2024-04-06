using Tingle.Extensions.Mustache.Parsing;

namespace Tingle.Extensions.Mustache.Tests;

public class TemplateParserTests
{
    [Theory]
    [InlineData("{{#Collection}}Collection has elements{{^Collection}}Collection doesn't have elements{{/Collection}}")]
    [InlineData("{{^Collection}}Collection doesn't have elements{{#Collection}}Collection has elements{{/Collection}}")]
    public void CanProcessCompoundConditionalGroup(string src)
    {
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        parser.Parse(src);
    }

    [Fact]
    public void CanProcessHandleMultilineTemplates()
    {
        var src = @"{{^Collection}}Collection doesn't have
                            elements{{#Collection}}Collection has
                        elements{{/Collection}}";
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        parser.Parse(src);
    }

    [Fact]
    public void ThrowsAnExceptionWhenConditionalGroupsAreMismatched()
    {
        var src = "{{#Collection}}Collection has elements{{/AnotherCollection}}";
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        var ex = Assert.Throws<MustacheParsingException>(() => parser.Parse(src));
        Assert.Equal("It appears that open and closing elements are mismatched.", ex.Message);
        Assert.Equal(new CharacterLocation(1, 39), ex.Location);
    }

    [Fact]
    public void CanProcessSimpleConditionalGroup()
    {
        var src = "{{#Collection}}Collection has elements{{/Collection}}";
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        parser.Parse(src);
    }

    [Fact]
    public void CanProcessSimpleNegatedCondionalGroup()
    {
        var src = "{{^Collection}}Collection has no elements{{/Collection}}";
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        parser.Parse(src);
    }

    [Fact]
    public void CanProcessSimpleValuePath()
    {
        var src = "Hello {{Name}}!";
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        parser.Parse(src);
    }

    [Fact]
    public void CanProcessComplexValuePath()
    {
        var src = "{{#content}}Hello {{../Person.Name}}!{{/content}}";
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        parser.Parse(src);
    }

    [Fact]
    public void CanProcessEachConstruct()
    {
        var src = "{{#each ACollection}}{{.}}{{/each}}";
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        parser.Parse(src);
    }

    [Theory]
    [InlineData("{{#ACollection}}{{.}}{{/each}}", 1, 22)]
    [InlineData("{{#ACollection}}{{.}}{{/ACollection}}{{/each}}", 1, 38)]
    [InlineData("{{/each}}", 1, 1)]
    public void ThrowsAnExceptionWhenEachIsMismatched(string src, int line, int character)
    {
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        var ex = Assert.Throws<MustacheParsingException>(() => parser.Parse(src));
        Assert.Equal("An 'each' block is being closed, but no corresponding opening element ('{{#each <name>}}') was detected.", ex.Message);
        Assert.Equal(new CharacterLocation(line, character), ex.Location);
    }

    [Fact]
    public void ThrowsParserExceptionForEmptyEach()
    {
        var src = "{{#each}}";
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        var ex = Assert.Throws<MustacheParsingException>(() => parser.Parse(src));
        Assert.Equal("The 'each' block being opened requires a model path to be specified in the form '{{#each <name>}}'.", ex.Message);
        Assert.Equal(new CharacterLocation(1, 1), ex.Location);
    }

    [Fact]
    public void ThrowsParserExceptionForEachWithoutPath()
    {
        var src = "{{#eachs}}{{name}}{{/each}}";
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        var ex = Assert.Throws<MustacheParsingException>(() => parser.Parse(src));
        Assert.Equal("The 'each' block being opened requires a model path to be specified in the form '{{#each <name>}}'.", ex.Message);
        Assert.Equal(new CharacterLocation(1, 1), ex.Location);
    }

    [Theory]
    [InlineData("{{#each element}}{{name}}")]
    [InlineData("{{#element}}{{name}}")]
    [InlineData("{{^element}}{{name}}")]
    public void ThrowsParserExceptionForUnclosedGroups(string src)
    {
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        var ex = Assert.Throws<MustacheParsingException>(() => parser.Parse(src));
        Assert.Equal("A scope block to the following path was opened but not closed: 'element', please close it using the appropriate syntax.", ex.Message);
        Assert.Equal(new CharacterLocation(1, 1), ex.Location);
    }

    [Theory]
    [InlineData("{{.../asdf.content}}", ".../asdf.content")]
    [InlineData("{{./}}", "./")]
    [InlineData("{{.. }}", "..")]
    [InlineData("{{..}}", "..")]
    [InlineData("{{@}}", "@")]
    [InlineData("{{[}}", "[")]
    [InlineData("{{]}}", "]")]
    [InlineData("{{)}}", ")")]
    [InlineData("{{(}}", "(")]
    [InlineData("{{~}}", "~")]
    [InlineData("{{$}}", "$")]
    [InlineData("{{%}}", "%")]
    public void ThrowsParserExceptionForInvalidPaths(string src, string path)
    {
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        var ex = Assert.Throws<MustacheParsingException>(() => parser.Parse(src));
        Assert.Equal($"The path '{path}' is not valid. Please see documentation for examples of valid paths.", ex.Message);
        Assert.Equal(new CharacterLocation(1, 1), ex.Location);
    }

    [Theory]
    [InlineData("{{/}}")]
    [InlineData("{{//}}")]
    public void ThrowsParserExceptionForMismatchedPaths(string src)
    {
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        var ex = Assert.Throws<MustacheParsingException>(() => parser.Parse(src));
        Assert.Equal("It appears that open and closing elements are mismatched.", ex.Message);
        Assert.Equal(new CharacterLocation(1, 1), ex.Location);
    }

    [Theory]
    [InlineData("{{first_name}}")]
    [InlineData("{{company.name}}")]
    [InlineData("{{company.address_line_1}}")]
    [InlineData("{{name}}")]
    public void DoesNotThrowForValidPath(string src)
    {
        var options = new TemplateParserOptions { };
        var parser = new TemplateParser(options);
        parser.Parse(src);
    }

    [Fact]
    public void ErrorsHaveSourceNamesSet()
    {
        var expectedSourceName = "TestBase";
        var src = "Hello, {{##each}}!!!";
        var parser = new TemplateParser(new TemplateParserOptions { SourceName = expectedSourceName, });

        var ex = Assert.Throws<MustacheParsingException>(() => parser.Parse(src));
        Assert.Equal(expectedSourceName, ex.SourceName);
    }
}
