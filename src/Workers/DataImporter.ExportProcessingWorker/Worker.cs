using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataImporter.Common.Models;
using DataImporter.Core.Services;

namespace DataImporter.ExportProcessingWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IExportService _exportService;

        public Worker(ILogger<Worker> logger, IExportService exportService)
        {
            _logger = logger;
            _exportService = exportService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var elapsedTime = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                if ((elapsedTime += 1000) == 1000 * 60 * 5)
                {
                    _logger.LogInformation("ExportProcessingWorker running...");
                    elapsedTime = 0;
                }
                try
                {
                    await _exportService.ProcessExportQueue();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "ExportProcessingWorker Error");
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
