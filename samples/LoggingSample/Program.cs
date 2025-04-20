var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddCliConsole(); // pulls from config

// // or without config
// builder.Logging.AddCliConsole(options =>
// {
//   options.SingleLine = true;
//   options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
// });

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();

class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
