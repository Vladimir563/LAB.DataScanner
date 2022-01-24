using Microsoft.Extensions.Configuration;
using System;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.Converters;

namespace LAB.DataScanner.HtmlToJsonConverter
{
    internal class Program
    {
        static void Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var rmqPublisher = new RmqPublisherBuilder()
                .UsingConfigExchangeAndRoutingKey(configuration.GetSection("Binding"))
                .UsingDefaultConnectionSetting()
                .Build();

            var rmqConsumer = new RmqConsumerBuilder(configuration.GetSection("Binding"))
                .UsingConfigQueueName(configuration.GetSection("Binding"))
                .UsingConfigConnectionSettings(configuration.GetSection("ConnectionSettings"))
                .Build();

            var htmlToJsonConverterEngine = new HtmlToJsonConverterEngine(configuration, rmqPublisher, rmqConsumer);

            htmlToJsonConverterEngine.Start();

            Console.ReadKey();

            //features / featureName for azure
        }
    }
}
