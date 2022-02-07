using LAB.DataScanner.Components.Interfaces.Persisters;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.HtmlToJsonConverter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
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

            var serviceProvider = BuildServiceProvider(new ServiceCollection(), configuration);

            var dbWorker = serviceProvider.GetService<IPersisterWorker>();

            dbWorker.StartConfiguring();

            dbWorker.Start();

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
                .AddSingleton<IRmqConsumer>(c => new RmqConsumerBuilder(configuration)
                    .UsingConfigQueueName(configuration.GetSection("Binding"))
                    .UsingConfigConnectionSettings(configuration.GetSection("ConnectionSettings"))
                    .UsingLogger(services.BuildServiceProvider().GetRequiredService<ILogger<RmqConsumerBuilder>>())
                    .Build())
                .AddSingleton<IPersisterWorker, DBPersisterWorker>()
                .BuildServiceProvider();
        }
    }
}