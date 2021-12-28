using LAB.DataScanner.Components.Services.MessageBroker;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;

namespace LAB.DataScanner.RabbitMQ.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile(@"F:\VS2022_source\repos\LAB.DataScanner\LAB.DataScanner.RabbitMQ.Test\config.json")
            .Build();

            var customer = new RmqConsumerBuilder()
                .UsingConfigQueueName(config.GetSection("Binding")).UsingConfigConnectionSettings(config.GetSection("ConnectionSettings")).Build();

            customer.StartListening((model, ea) =>
            {
                var body = ea.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);

                var routingKey = ea.RoutingKey;

                customer.Ack(ea);

                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey,
                                  message);
            });

            Console.ReadKey();
        }
    }
}
