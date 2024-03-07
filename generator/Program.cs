using StaticDataGenerator;

// We use a generator project instead of source generators for various reason:
// 1. The data does not change as much (no need to perform source generation every time)
// 2. The static data is large and slows down during source generation
// 3. This project can be tuned to download new data which required lots more work with source generators
// 4. We can generate specific JSON files for use in our frontend apps (JS/TS)

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<FileDownloader>();

builder.Services.AddSingleton<IGenerator, CountriesGenerator>();
builder.Services.AddSingleton<IGenerator, CurrenciesGenerator>();
builder.Services.AddSingleton<IGenerator, LanguagesGenerator>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Generate code
var generators = app.Services.GetRequiredService<IEnumerable<IGenerator>>().ToList();
logger.LogInformation("Found {GeneratorsCount} generators", generators.Count);
foreach (var generator in generators)
{
    var outputPath = GetRelativePath(generator.OutputFilePath, true, "../");
    var jsonPath = GetRelativePath(generator.OutputFilePathJson, false);

    if (jsonPath is not null)
        logger.LogInformation("Generating from {DataFileName} to {OutputPath} and {JsonPath}", generator.DataFileName, outputPath, jsonPath);
    else
        logger.LogInformation("Generating from {DataFileName} to {OutputPath}", generator.DataFileName, outputPath);
    await generator.GenerateAsync();
}

logger.LogInformation("Finished!");


static string? GetRelativePath(string? path, bool createDirectory = false, string combinePath = "")
{
    if (path is null) return null;

    var dir = Path.GetDirectoryName(path)!;
    if (createDirectory && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

    return Path.GetRelativePath(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), combinePath)), path);
}
