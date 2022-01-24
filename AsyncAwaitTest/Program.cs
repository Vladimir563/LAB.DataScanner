using LAB.DataScanner.Components.Services.Converters;
using LAB.DataScanner.Components.Services.Downloaders;
using LAB.DataScanner.Components.Services.MessageBroker;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitTest
{
    internal class Program
    {
        static void Factorial()
        {
            Thread.Sleep(3000);
        }
        // определение асинхронного метода
        static async void FactorialAsync()
        {
            Console.WriteLine("Начало метода FactorialAsync"); // выполняется синхронно
            await Task.Run(() => Factorial());                // выполняется асинхронно
            Console.WriteLine("Конец метода FactorialAsync");
        }

        static void Main(string[] args)
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

            HttpDataRetriever dataRetriever = new HttpDataRetriever(new HttpClient(), configuration);

            HtmlToJsonConverterEngine htmlToJsonConverter = new HtmlToJsonConverterEngine
                (configuration, rmqPublisher, rmqConsumer);


            List<string> urlsList = new List<string>()
            {
                "https://www.epam.com/careers/job-listings?query=1&country=Russia",
                "https://www.epam.com/careers/job-listings?query=2&country=Russia",
                "https://www.epam.com/careers/job-listings?query=3&country=Russia",
                "https://www.epam.com/careers/job-listings?query=4&country=Russia",
                "https://www.epam.com/careers/job-listings?query=5&country=Russia"
            };

            Stack <string> stack = new Stack<string>();

            for (int i = 0; i < urlsList.Count; i++)
            {
                //dataRetriever.RetrieveDynamicDataAsync(urlsList[i], stack);
            }

            for (int i = 0; i < 20; i++)
            {
                if (stack.Count > 0) 
                {
                    var t = htmlToJsonConverter.ParseHtmlToJson(stack.Pop());
                    Console.WriteLine("\n\n\n\n****************************************\n\n\n\n");
                }
                Thread.Sleep(5000);
            }

            Console.WriteLine("End of programm");
        }
    }
}
