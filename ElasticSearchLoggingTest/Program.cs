using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;

namespace ElasticSearchLoggingTest
{
    internal class Program
    {
        static void Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("../../../logs/txt/log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearchUrl"]))
                {
                    AutoRegisterTemplate = true,
                    BufferBaseFilename = @"../../../logs/buffer",
                    CustomFormatter = new ElasticsearchJsonFormatter(),
                    IndexFormat = $"Service_{configuration["ServiceName"]}"
                })
                .ReadFrom.Configuration(configuration)
                .CreateLogger();


            for (int i = 0; i < 3; i++)
            {
                logger.Warning("{@Warning}!!!", 2903);

                logger.Information("{@Info}!", 999);

                logger.Debug("{@Debug}", 190);

                logger.Error("{@Error}", 404);

                logger.Verbose("{@Verbose}", 9000);

                logger.Fatal("{@Fatal}", 666);
            }

            Console.ReadKey();

            //create a setting class and bind with 
        }
    }
}
