using Microsoft.Extensions.Configuration;
using System;
using LAB.DataScanner.Components.Services.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using LAB.DataScanner.Components.Interfaces.Converters;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Services.Converters;
using LAB.DataScanner.Components.Settings;
using LAB.DataScanner.Components.Services.Validators;
using LAB.DataScanner.Components.Interfaces.Engines;

namespace LAB.DataScanner.HtmlToJsonConverter
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

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IRmqPublisher>(rqmPublisher)
                .AddSingleton<IRmqConsumer>(rmqConsumer)
                .AddSingleton<HtmlToJsonConverterSettings>(converterSettings)
                .AddSingleton<RmqPublisherSettings>(rmqPublisherSettings)
                .AddSingleton<RmqConsumerSettings>(rqmConsumerSettings)
                .AddSingleton<IEngine, HtmlToJsonConverterEngine>()
                .BuildServiceProvider();

            var htmlToJsonConverter = serviceProvider.GetService<IEngine>();

            htmlToJsonConverter.Start();

            Console.ReadKey();
        }
    }
}
