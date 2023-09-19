var host = Host.CreateDefaultBuilder(args)
               .UseSerilog(builder =>
               {
                   builder.ConfigureSensitiveDataMasking(options =>
                   {
                       options.ExcludeProperties.Add("SomeNecessaryProperty");
                   });
               })
               .ConfigureServices(services =>
               {
                   services.AddHostedService<Worker>();
               })
               .Build();

await host.RunAsync();

class Worker : BackgroundService
{
    private readonly ILogger logger;

    public Worker(ILogger<Worker> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time:R}", DateTimeOffset.Now);
            await Task.Delay(2000, stoppingToken);
        }
    }
}
