using System.Diagnostics;
using System.Fabric;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using LAB.DataScanner.Components;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Settings;
using LAB.DataScanner.Components.Services.Validators;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Interfaces.Engines;
using LAB.DataScanner.HtmlToJsonConverter;

namespace LAB.DataScanner.SimpleTableDBPersister
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {

                ServiceRuntime.RegisterServiceAsync("LAB.DataScanner.SimpleTableDBPersisterType", context => new SimpleTableDBPersister
                    (context, GetLogger(), GetServiceProvider(context))).GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static Serilog.ILogger GetLogger()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Debug()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                    IndexFormat = "simple_table_db_persister_service",
                    MinimumLogEventLevel = Serilog.Events.LogEventLevel.Verbose
                })
                .CreateLogger();

            return logger;
        }

        private static ServiceProvider GetServiceProvider(StatelessServiceContext context)
        {

            #region Configuration, settings binding and validation

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(context)
                .Build();

            var dbPersisterSettings = new SimpleTableDBPersisterSettings();

            configuration.GetSection("ConnectionSettings").Bind(dbPersisterSettings);

            configuration.GetSection("DBTableCreationSettings").Bind(dbPersisterSettings);

            SettingsValidator.Validate(dbPersisterSettings);

            var rqmConsumerSettings = new RmqConsumerSettings();

            configuration.GetSection("ConsumerSettings").Bind(rqmConsumerSettings);

            SettingsValidator.Validate(rqmConsumerSettings);
            #endregion

            var rmqConsumer = new RmqConsumerBuilder(rqmConsumerSettings)
                .UsingConfigQueueName(rqmConsumerSettings)
                .UsingConfigConnectionSettings(dbPersisterSettings)
                .Build();

            return new ServiceCollection()
                .AddSingleton<IRmqConsumer>(rmqConsumer)
                .AddSingleton<SimpleTableDBPersisterSettings>(dbPersisterSettings)
                .AddSingleton<RmqConsumerSettings>(rqmConsumerSettings)
                .AddSingleton<IEngine, DBPersisterEngine>()
                .BuildServiceProvider();

        }
    }
}
