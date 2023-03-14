using Tingle.Extensions.Http.Authentication;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient(nameof(Worker))
                .AddApiKeyHeaderAuthenticationHandler("my-awesome-key");

builder.Services.AddHttpClient($"{nameof(Worker)}2")
                .AddApiKeyQueryAuthenticationHandler("my-api-key-here", queryParameterName: "key");

builder.Services.AddHttpClient($"{nameof(Worker)}3")
                .AddSharedKeyAuthenticationHandler("my-base-64-encoded-key", scheme: "Bearer");

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpClient($"{nameof(Worker)}4")
                .AddAuthenticationHandler(provider => new OAuthClientCredentialHandler
                {
                    Scheme = "Bearer",

                    // set OAuth values to match your scenario
                    AuthenticationEndpoint = "https://oauth-1.contoso.com",
                    Resource = "https://api.contoso.com",
                    ClientId = "awesome-app-id",
                    ClientSecret = "super-secret",

                    Logger = provider.GetRequiredService<ILogger<Program>>(), // optional, useful for debugging

                    // caching can be disabled by setting either CacheKey or Cache to null
                    CacheKey = $"{nameof(Worker)}4:auth-token",
                    // either IMemoryCache or IDistributedCache
                    Cache = new(provider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()),
                    //Cache = new(provider.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>()),
                });

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpClient($"{nameof(Worker)}5")
                .AddAuthenticationHandler(provider => new AzureAdB2BHandler
                {
                    Scheme = "Bearer",

                    // set OAuth values to match your scenario
                    TenantId = "00000000-0000-1111-0001-000000000000",
                    Resource = "https://api.contoso.com",
                    ClientId = "awesome-app-id",
                    ClientSecret = "super-secret",

                    Logger = provider.GetRequiredService<ILogger<Program>>(), // optional, useful for debugging

                    // caching can be disabled by setting either CacheKey or Cache to null
                    CacheKey = $"{nameof(Worker)}5:auth-token",
                    // either IMemoryCache or IDistributedCache
                    Cache = new(provider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()),
                    //Cache = new(provider.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>()),
                });

var host = builder.Build();

await host.RunAsync();

class Worker : BackgroundService
{
    private readonly HttpClient httpClient;
    private readonly ILogger logger;

    public Worker(IHttpClientFactory httpClientFactory, ILogger<Worker> logger)
    {
        httpClient = httpClientFactory?.CreateClient(nameof(Worker)) ?? throw new ArgumentNullException(nameof(httpClientFactory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            await httpClient.GetAsync("https://contoso.com", stoppingToken);
        }
    }
}
