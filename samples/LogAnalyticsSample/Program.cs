using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LogAnalyticsSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                      .ConfigureLogging((context, loggingBuilder) =>
                      {
                          loggingBuilder.AddLogAnalytics(context.Configuration.GetSection("LogAnalytics"));
                      })
                      .ConfigureServices((context, services) =>
                      {
                          services.AddHostedService<TestService>();
                      })
                      .RunConsoleAsync();
        }

    }

    class TestService : BackgroundService
    {
        private readonly ILogger logger;

        public TestService(ILogger<TestService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for (var i = 1; i <= 5; i++)
            {
                logger.LogInformation("In am working on something. Iteration {IterationNumber}", i);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
