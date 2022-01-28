using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.HtmlToJsonConverter;
using Microsoft.Extensions.Configuration;
using System;

namespace LAB.DataScanner.SimpleTableDBPersister
{
    internal class Program
    {
        static void Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var rmqConsumer = new RmqConsumerBuilder(configuration.GetSection("Binding"))
                .UsingConfigQueueName(configuration.GetSection("Binding"))
                .UsingConfigConnectionSettings(configuration.GetSection("ConnectionSettings"))
                .Build();

            DBPersisterWorker dbWorker = new DBPersisterWorker(configuration, rmqConsumer);

            dbWorker.StartConfiguring();

            dbWorker.Start();

            Console.ReadKey();
        }
    }
}