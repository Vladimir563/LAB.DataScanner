using System.Fabric;
using LAB.DataScanner.Components.Interfaces.Engines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using Serilog.Core.Enrichers;

namespace LAB.DataScanner.HtmlToJsonConverter
{
    internal sealed class HtmlToJsonConverter : StatelessService
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        private readonly ServiceProvider _serviceProvider;

        public HtmlToJsonConverter(StatelessServiceContext context, Serilog.ILogger serilog, ServiceProvider serviceProvider)
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

            _logger = new LoggerFactory().AddSerilog(serilog.ForContext(properties)).CreateLogger<HtmlToJsonConverter>();

            _serviceProvider = serviceProvider;
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation("The \"HtmlToJsonConverter\" service will be started soon...");

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
