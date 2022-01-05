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

            var applicationSection = configuration.GetSection("Application");

            var bindingSection = configuration.GetSection("Binding");

            var rmqPublisher = new RmqPublisherBuilder()
                        .UsingDefaultConnectionSetting().Build();

            var service = new UrlsGeneratorEngine(rmqPublisher, applicationSection, bindingSection);

            service.Start();
        }
    }
}
