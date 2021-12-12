using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataImporter.Common.Models;
using DataImporter.Core.Services;

namespace DataImporter.ImportProcessingWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IImportService _importService;

        public Worker(ILogger<Worker> logger, IImportService importService)
        {
            _logger = logger;
            _importService = importService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var elapsedTime = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                if ((elapsedTime += 1000) == 1000 * 60 * 5)
                {
                    _logger.LogInformation("ImportProcessingWorker running...");
                    elapsedTime = 0;
                }
                try
                {
                    await _importService.ProcessImportQueueAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "ImportProcessingWorker Error");
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
