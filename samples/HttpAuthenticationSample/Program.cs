var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient(nameof(Worker))
                .AddApiKeyHeaderAuthenticationHandler("my-awesome-key");

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

            await httpClient.GetAsync("https://tingle.software", stoppingToken);
        }
    }
}
