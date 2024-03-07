﻿using System.CodeDom.Compiler;
using System.Text;
using System.Text.Json;

namespace StaticDataGenerator;

public interface IGenerator
{
    public string OutputFilePath { get; }
    public string DataFileName { get; }
    public string? OutputFilePathJson { get; }
    Task GenerateAsync(CancellationToken cancellationToken = default);
}

public abstract class AbstractGenerator<T> : IGenerator where T : class, new()
{
    private const string Header = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the StaticDataGenerator source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
";

    private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IHostEnvironment environment;
    private readonly FileDownloader downloader;

    protected AbstractGenerator(IHostEnvironment environment, FileDownloader downloader, string dataFileName)
    {
        this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
        this.downloader = downloader ?? throw new ArgumentNullException(nameof(downloader));
        DataFileName = dataFileName ?? throw new ArgumentNullException(nameof(dataFileName));
        ClassName = GetType().Name.Replace("CodeGenerator", "").Replace("Generator", "");

        var targetDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), $"../src/Tingle.Extensions.Primitives/Generated"));
        OutputFilePath = Path.Combine(targetDirectory, $"{ClassName}.g.cs");
        OutputFilePathJson = environment.ContentRootFileProvider.GetFileInfo($"{ClassName.ToLower()}.json").PhysicalPath;
    }

    public string ClassName { get; }

    public string OutputFilePath { get; }

    public string DataFileName { get; }

    public string? OutputFilePathJson { get; }

    public async Task GenerateAsync(CancellationToken cancellationToken = default)
    {
        var data = (await ParseAsync(cancellationToken)) ?? throw new InvalidOperationException("Deserialization failed");

        var sb = new StringBuilder();
        var writer = new IndentedTextWriter(new StringWriter(sb));

        writer.WriteLine(Header);

        writer.WriteLine("using System.Collections;");
        writer.WriteLine("using System.Collections.Generic;");

        // write the namespace
        writer.WriteLine();
        writer.WriteLine("namespace Tingle.Extensions.Primitives;");
        writer.WriteLine();

        // begin the class
        writer.WriteLine($"internal partial class {ClassName}");
        writer.WriteLine("{");

        // write the data
        writer.Indent++;
        WriteCode(writer, data);
        writer.Indent--;

        // end the class
        writer.WriteLine("}");
        await writer.FlushAsync(cancellationToken);

        // output to file
        await File.WriteAllTextAsync(OutputFilePath, sb.ToString(), Encoding.UTF8, cancellationToken);

        // generate the JSON file
        if (OutputFilePathJson is not null)
        {
            sb.Clear();
            writer = new IndentedTextWriter(new StringWriter(sb), "  "); // only 2 spaces for JSON
            WriteJson(writer, data);
            await writer.FlushAsync(cancellationToken);
            if (sb.Length > 0)
            {
                await File.WriteAllTextAsync(OutputFilePathJson, sb.ToString(), Encoding.UTF8, cancellationToken);
            }
        }
    }

    protected virtual async ValueTask<T?> ParseAsync(CancellationToken cancellationToken = default)
    {
        var fi = environment.ContentRootFileProvider.GetFileInfo(Path.Combine("Files", DataFileName));
        using var dataStream = fi.CreateReadStream();
        return await ParseAsync(dataStream, cancellationToken);
    }

    protected virtual ValueTask<T?> ParseAsync(Stream dataStream, CancellationToken cancellationToken = default)
        => JsonSerializer.DeserializeAsync<T>(dataStream, serializerOptions, cancellationToken);

    protected abstract void WriteCode(IndentedTextWriter writer, T data);
    protected virtual void WriteJson(IndentedTextWriter writer, T data) { }
}