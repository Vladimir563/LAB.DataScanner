using LAB.DataScanner.Components.Services;
using LAB.DataScanner.Components.Services.Converters;
using LAB.DataScanner.Components.Services.Downloaders;
using LAB.DataScanner.Components.Services.MessageBroker;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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


            var rmqConsumer = new RmqConsumerBuilder(configuration.GetSection("Binding"))
                .UsingConfigQueueName(configuration.GetSection("Binding"))
                .UsingConfigConnectionSettings(configuration.GetSection("ConnectionSettings"))
                .Build();

            var dataRetriever = new HttpDataRetriever(new HttpClient(), configuration);

            var webPageDownloaderEngine = new WebPageDownloaderEngine
                (configuration.GetSection("Binding"),
                dataRetriever, rmqPublisher, rmqConsumer,
                new UrlsValidator());

            webPageDownloaderEngine.Start();

            Console.ReadKey();
        }
    }
}
