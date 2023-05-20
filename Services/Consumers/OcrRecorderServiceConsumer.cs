﻿// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OCRStudio.Interfaces;

namespace OCRStudio.Services.Consumers
{
    internal sealed partial class OcrRecorderServiceConsumer : BackgroundService, IHostedService
    {
        private readonly ILogger<OcrRecorderService> _logger;

        public IServiceProvider Services { get; }

        public OcrRecorderServiceConsumer(IServiceProvider services, ILogger<OcrRecorderService> logger)
        {
            Services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting OCR service...");

            await DoWork(stoppingToken);
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            using IServiceScope scope = Services.CreateScope();
            IScopedProcessingService service = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

            await service.DoWork(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stopping OCR service...");

            await base.StopAsync(stoppingToken);
        }
    }
}
