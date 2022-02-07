using LAB.DataScanner.Components.Interfaces.Generators;
using LAB.DataScanner.Components.Services.Generators;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

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

            var serviceProvider = BuildServiceProvider(new ServiceCollection(), configuration);

            var urlsGeneratorEngine = serviceProvider.GetService<IGeneratorEngine>();

            urlsGeneratorEngine.Generate();

            Console.ReadKey();
        }

        private static ServiceProvider BuildServiceProvider(IServiceCollection services, IConfigurationRoot configuration)
        {
            return services
                .AddLogging(builder =>
                {
                    var logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();
                    builder.AddSerilog(logger);
                })
                .AddSingleton<IConfigurationRoot>(c => configuration)
                .AddSingleton<IRmqPublisher>(r => new RmqPublisherBuilder()
                    .UsingConfigExchangeAndRoutingKey(configuration.GetSection("Binding"))
                    .UsingDefaultConnectionSetting()
                    .UsingLogger(services.BuildServiceProvider().GetRequiredService<ILogger<RmqPublisherBuilder>>())
                    .Build())
                .AddSingleton<IGeneratorEngine, UrlsGeneratorEngine>()
                .BuildServiceProvider();
        }
    }
}
