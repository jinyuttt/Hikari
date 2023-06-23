using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp3
{
    public class PrintTimeService : BackgroundService
    {
        private readonly ILogger _logger;

        public PrintTimeService(ILogger<PrintTimeService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("The current time is: {CurrentTime}", DateTimeOffset.UtcNow);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
