using LAB.DataScanner.Components.Services.Generators;
using LAB.DataScanner.Components.Services.MessageBroker;
using Microsoft.Extensions.Configuration;

namespace LAB.DataScanner.UrlsGenerator
{
    internal class Program
    {
        static void Main()
        {
            var configuration = new ConfigurationBuilder()
                        .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

            var bindingSection = configuration.GetSection("Binding");

            var rmqPublisher = new RmqPublisherBuilder()
                .UsingConfigExchangeAndRoutingKey(bindingSection)
                .UsingDefaultConnectionSetting().Build();

            var service = new UrlsGeneratorEngine(rmqPublisher, configuration);

            service.Start();
        }
    }
}
