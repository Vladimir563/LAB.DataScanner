using Microsoft.Extensions.Configuration;
using System;
using LAB.DataScanner.Components.Services.MessageBroker;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using LAB.DataScanner.Components.Interfaces.Converters;
using Serilog;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Logging;
using LAB.DataScanner.Components.Services.Converters;

namespace LAB.DataScanner.HtmlToJsonConverter
{
    internal class Program
    {
        static void Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var serviceProvider = BuildServiceProvider(new ServiceCollection(), configuration);

            var htmlToJsonConverterEngine = serviceProvider.GetService<IConverterEngine<string, string>>();

            htmlToJsonConverterEngine.Start();

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
                .AddSingleton<IRmqConsumer>(c => new RmqConsumerBuilder(configuration)
                    .UsingConfigQueueName(configuration.GetSection("Binding"))
                    .UsingConfigConnectionSettings(configuration.GetSection("ConnectionSettings"))
                    .UsingLogger(services.BuildServiceProvider().GetRequiredService<ILogger<RmqConsumerBuilder>>())
                    .Build())
                .AddSingleton<IConverterEngine<string, string>, HtmlToJsonConverterEngine>()
                .BuildServiceProvider();
        }
    }
}
