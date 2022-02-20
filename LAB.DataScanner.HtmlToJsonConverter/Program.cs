using System.Fabric;
using LAB.DataScanner.Components;
using LAB.DataScanner.Components.Interfaces.Engines;
using LAB.DataScanner.Components.Services.Converters;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Services.Validators;
using LAB.DataScanner.Components.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace LAB.DataScanner.HtmlToJsonConverter
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                ServiceRuntime.RegisterServiceAsync("LAB.DataScanner.HtmlToJsonConverterType", context => new HtmlToJsonConverter
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
                    IndexFormat = "html_to_json_converter_service",
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

            var converterSettings = new HtmlToJsonConverterSettings();

            configuration.GetSection("Application").Bind(converterSettings);

            configuration.GetSection("ConnectionSettings").Bind(converterSettings);

            configuration.GetSection("HtmlDataDownloadingSettingsArrs").Bind(converterSettings);

            configuration.GetSection("DBTableCreationSettings").Bind(converterSettings);

            SettingsValidator.Validate(converterSettings);

            var rmqPublisherSettings = new RmqPublisherSettings();

            configuration.GetSection("PublisherSettings").Bind(rmqPublisherSettings);

            SettingsValidator.Validate(rmqPublisherSettings);

            var rqmConsumerSettings = new RmqConsumerSettings();

            configuration.GetSection("ConsumerSettings").Bind(rqmConsumerSettings);

            SettingsValidator.Validate(rqmConsumerSettings);
            #endregion

            var rqmPublisher = new RmqPublisherBuilder()
                .UsingConfigExchangeAndRoutingKey(rmqPublisherSettings)
                .UsingDefaultConnectionSetting()
                .Build();

            var rmqConsumer = new RmqConsumerBuilder(rqmConsumerSettings)
                .UsingConfigQueueName(rqmConsumerSettings)
                .UsingConfigConnectionSettings(converterSettings)
                .Build();

            return new ServiceCollection()
                 .AddSingleton<IRmqPublisher>(rqmPublisher)
                 .AddSingleton<IRmqConsumer>(rmqConsumer)
                 .AddSingleton<HtmlToJsonConverterSettings>(converterSettings)
                 .AddSingleton<RmqPublisherSettings>(rmqPublisherSettings)
                 .AddSingleton<RmqConsumerSettings>(rqmConsumerSettings)
                 .AddSingleton<IEngine, HtmlToJsonConverterEngine>()
                 .BuildServiceProvider();
        }
    }
}
