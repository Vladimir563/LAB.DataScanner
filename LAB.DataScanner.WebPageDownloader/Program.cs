using LAB.DataScanner.Components.Interfaces.Engines;
using LAB.DataScanner.Components.Services.Downloaders;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Services.Validators;
using LAB.DataScanner.Components.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LAB.DataScanner.WebPageDownloader
{
    internal class Program
    {
        static void Main()
        {
            #region Configuration, settings binding and validation
            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
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

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IRmqPublisher>(rqmPublisher)
                .AddSingleton<IRmqConsumer>(rmqConsumer)
                .AddSingleton<RmqPublisherSettings>(rmqPublisherSettings)
                .AddSingleton<RmqConsumerSettings>(rqmConsumerSettings)
                .AddSingleton<WebPageDownloaderSettings>(downloaderSettings)
                .AddSingleton<IDataRetriever, HttpDataRetriever>()
                .AddSingleton<IEngine, WebPageDownloaderEngine>()
                .BuildServiceProvider();

            var webPageDownloaderEngine = serviceProvider.GetService<IEngine>();

            webPageDownloaderEngine.Start();

            Console.ReadKey();
        }
    }
}
