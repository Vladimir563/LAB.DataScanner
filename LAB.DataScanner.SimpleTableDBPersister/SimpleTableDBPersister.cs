using LAB.DataScanner.Components.Interfaces.Engines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using Serilog.Core.Enrichers;
using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace LAB.DataScanner.SimpleTableDBPersister
{
    internal sealed class SimpleTableDBPersister : StatelessService
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        private readonly ServiceProvider _serviceProvider;

        public SimpleTableDBPersister(StatelessServiceContext context, Serilog.ILogger serilog, ServiceProvider serviceProvider)
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

            _logger = new LoggerFactory().AddSerilog(serilog.ForContext(properties)).CreateLogger<SimpleTableDBPersister>();

            _serviceProvider = serviceProvider;
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation("The \"SimpleTableDBPersister\" service will be started soon...");

                    var simpleTableDBPersister = _serviceProvider.GetService<IEngine>();

                    simpleTableDBPersister?.Start();
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
