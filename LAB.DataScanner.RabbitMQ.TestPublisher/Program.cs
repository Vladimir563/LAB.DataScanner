using LAB.DataScanner.Components.Services.MessageBroker;
using System;
using System.Text;

namespace LAB.DataScanner.RabbitMQ.TestPublisher
{
    internal class Program
    {
        static void Main()
        {
            RmqPublisherBuilder publisherBuilder = new RmqPublisherBuilder();

            var publisher = publisherBuilder.UsingExchangeAndRoutingKey("UrlsGeneratorExchange", "#")
                .UsingDefaultConnectionSetting()
                .Build();

            while (true) 
            {
                Console.WriteLine("Type a message for broker...");

                var mes = Console.ReadLine();

                if (mes.Equals("q")) break;

                byte[] body = Encoding.UTF8.GetBytes(mes);

                publisher.Publish(body);
            }
        }
    }
}
