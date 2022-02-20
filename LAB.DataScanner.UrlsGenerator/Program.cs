using LAB.DataScanner.Components.Interfaces.Engines;
using LAB.DataScanner.Components.Services.Generators;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Services.Validators;
using LAB.DataScanner.Components.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LAB.DataScanner.UrlsGenerator
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

            UrlsGeneratorSettings generatorSettings = new UrlsGeneratorSettings();

            configuration.GetSection("Application").Bind(generatorSettings);

            configuration.GetSection("ConnectionSettings").Bind(generatorSettings);

            SettingsValidator.Validate(generatorSettings);

            var rmqPublisherSettings = new RmqPublisherSettings();

            configuration.GetSection("PublisherSettings").Bind(rmqPublisherSettings);

            SettingsValidator.Validate(rmqPublisherSettings);

            #endregion

            var rmqPublisher = new RmqPublisherBuilder()
                .UsingConfigExchangeAndRoutingKey(rmqPublisherSettings)
                .UsingDefaultConnectionSetting()
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IRmqPublisher>(rmqPublisher)
                .AddSingleton<UrlsGeneratorSettings>(generatorSettings)
                .AddSingleton<RmqPublisherSettings>(rmqPublisherSettings)
                .AddSingleton<IEngine, UrlsGeneratorEngine>()
                .BuildServiceProvider();

            var urlsGeneratorEngine = serviceProvider.GetService<IEngine>();

            urlsGeneratorEngine.Start();
        }
    }
}
