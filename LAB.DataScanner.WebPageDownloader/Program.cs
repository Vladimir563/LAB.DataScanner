using System.Fabric;
using LAB.DataScanner.Components.Interfaces.Engines;
using LAB.DataScanner.Components.Services.Downloaders;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Services.Validators;
using LAB.DataScanner.Components.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;

namespace LAB.DataScanner.WebPageDownloader
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                try
                {
                    ServiceRuntime.RegisterServiceAsync("LAB.DataScanner.WebPageDownloaderType", context => new WebPageDownloader
                        (context, GetLogger(context), GetServiceProvider(context))).GetAwaiter().GetResult();

                    Thread.Sleep(Timeout.Infinite);
                }
                catch (Exception e)
                {
                    throw;
                }

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static Serilog.ILogger GetLogger(StatelessServiceContext context)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Debug()
                //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                //{
                //    AutoRegisterTemplate = true,
                //    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                //    IndexFormat = "urls_generator_service",
                //    MinimumLogEventLevel = Serilog.Events.LogEventLevel.Verbose
                //})
                .CreateLogger();

            return logger;
        }

        private static ServiceProvider GetServiceProvider(StatelessServiceContext context)
        {

            #region Configuration, settings binding and validation

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(context)
                .Build();

            var downloaderSettings = new WebPageDownloaderSettings();

            configuration.GetSection("Application").Bind(downloaderSettings);

            configuration.GetSection("ConnectionSettings").Bind(downloaderSettings);

            configuration.GetSection("HtmlDataDownloadingSettingsArrs").Bind(downloaderSettings);

            SettingsValidator.Validate(downloaderSettings);

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
                .UsingConfigConnectionSettings(downloaderSettings)
                .Build();

            return new ServiceCollection()
                .AddSingleton<IRmqPublisher>(rqmPublisher)
                .AddSingleton<IRmqConsumer>(rmqConsumer)
                .AddSingleton<RmqPublisherSettings>(rmqPublisherSettings)
                .AddSingleton<RmqConsumerSettings>(rqmConsumerSettings)
                .AddSingleton<WebPageDownloaderSettings>(downloaderSettings)
                .AddSingleton<IDataRetriever, HttpDataRetriever>()
                .AddSingleton<IEngine, WebPageDownloaderEngine>()
                .BuildServiceProvider();
        }
    }
}
