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

class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time:R}", DateTimeOffset.Now);
            await Task.Delay(2000, stoppingToken);
        }
    }
}
