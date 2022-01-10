using LAB.DataScanner.Components.Services;
using LAB.DataScanner.Components.Services.Downloaders;
using LAB.DataScanner.Components.Services.MessageBroker;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace LAB.DataScanner.WebPageDownloader
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


            var rmqConsumer = new RmqConsumerBuilder()
                .UsingConfigSettings(configuration.GetSection("Binding"))
                .UsingConfigConnectionSettings(configuration.GetSection("ConnectionSettings"))
                .Build();


            var dataRetriever = new HttpDataRetriever(new HttpClient());

            var engine = new WebPageDownloaderEngine(configuration.GetSection("Binding"), dataRetriever, rmqPublisher, rmqConsumer);

            engine.Start();

            Console.ReadKey();
        }
    }
}
