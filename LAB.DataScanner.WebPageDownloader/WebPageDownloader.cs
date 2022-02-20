using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LAB.DataScanner.Components.Interfaces.Engines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using Serilog.Core.Enrichers;

namespace LAB.DataScanner.WebPageDownloader
{

    internal sealed class WebPageDownloader : StatelessService
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        private readonly ServiceProvider _serviceProvider;

        public WebPageDownloader(StatelessServiceContext context, Serilog.ILogger serilog, ServiceProvider serviceProvider)
            : base(context)
        {
            PropertyEnricher[] properties = new PropertyEnricher[]
            {
                    new PropertyEnricher("ServiceTypeName", context.ServiceTypeName),
                    new PropertyEnricher("ServiceName", context.ServiceName),
                    new PropertyEnricher("PartitionId", context.PartitionId),
                    new PropertyEnricher("InstanceId", context.ReplicaOrInstanceId),
            };

            serilog.ForContext(properties);

            _logger = new LoggerFactory().AddSerilog(serilog.ForContext(properties)).CreateLogger<WebPageDownloader>();

            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    var webPageDownloaderEngine = _serviceProvider.GetService<IEngine>();

                    webPageDownloaderEngine?.Start();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    throw;
                }


            }, cancellationToken);
        }
    }
}
