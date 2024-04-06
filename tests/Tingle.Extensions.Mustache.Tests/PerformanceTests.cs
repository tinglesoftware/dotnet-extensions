﻿using System.Diagnostics;
using Tingle.Extensions.Mustache.Parsing;
using Tingle.Extensions.Mustache.Rendering;
using Xunit.Abstractions;

namespace Tingle.Extensions.Mustache.Tests;

public class PerformanceTests(ITestOutputHelper outputHelper)
{
    [Theory]
    [InlineData("Model Depth", 5, 30000, 10, 5000)]
    [InlineData("Model Depth", 10, 30000, 10, 5000)]
    [InlineData("Model Depth", 100, 30000, 10, 5000)]
    [InlineData("Substitutions", 5, 30000, 10, 5000)]
    [InlineData("Substitutions", 5, 30000, 50, 5000)]
    [InlineData("Substitutions", 5, 30000, 100, 5000)]
    [InlineData("Template Size", 5, 15000, 10, 5000)]
    [InlineData("Template Size", 5, 25000, 10, 5000)]
    [InlineData("Template Size", 5, 30000, 10, 5000)]
    [InlineData("Template Size", 5, 50000, 10, 5000)]
    [InlineData("Template Size", 5, 100000, 10, 5000)]
    public void TestRuns(string variation, int modelDepth, int sizeOfTemplate, int inserts, int runs)
    {
        var model = ConstructModelAndPath(modelDepth);
        var baseTemplate = Enumerable.Range(1, 5)
            .Aggregate("", (seed, current) => seed += " {{" + model.Item2 + "}}");
        while (baseTemplate.Length <= sizeOfTemplate)
        {
            baseTemplate += model.Item2 + "\r\n";
        }

        TemplateParsingResult? template = null;

        //make sure this class is JIT'd before we start timing.
        var parser = new TemplateParser(new TemplateParserOptions { });
        parser.Parse("asdf");

        var totalTime = Stopwatch.StartNew();
        var parseTime = Stopwatch.StartNew();
        for (var i = 0; i < runs; i++)
        {
            template = parser.Parse(baseTemplate);
        }

        parseTime.Stop();

        Assert.NotNull(template);

        var renderTime = Stopwatch.StartNew();
        for (var i = 0; i < runs; i++)
        {
            var renderer = new TemplateRenderer(template!.Value, new TemplateRenderingOptions { });
            renderer.Render(model.Item1);
        }

        renderTime.Stop();
        totalTime.Stop();
        outputHelper.WriteLine(
            "Variation: '{8}', Time/Run: {7}ms, Runs: {0}x, Model Depth: {1}, SubstitutionCount: {2}, Template Size: {3}, ParseTime: {4}, RenderTime: {5}, Total Time: {6}",
            runs, modelDepth, inserts, sizeOfTemplate, parseTime.Elapsed, renderTime.Elapsed, totalTime.Elapsed,
            totalTime.ElapsedMilliseconds / (double)runs, variation);
    }

    private Tuple<Dictionary<string, object?>, string> ConstructModelAndPath(int modelDepth, string? path = null)
    {
        path = Guid.NewGuid().ToString("n");
        var model = new Dictionary<string, object?>() { };

        if (modelDepth > 1)
        {
            var child = ConstructModelAndPath(modelDepth - 1, path);
            model[path] = child.Item1;
            path = path + "." + child.Item2;
        }

        return Tuple.Create(model, path);
    }
}
