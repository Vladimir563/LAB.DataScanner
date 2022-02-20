using LAB.DataScanner.Components.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LAB.DataScanner.WebPageDownloaderMicroService
{
    public class WebPageDownloaderWorker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            WebPageDownloaderSettings settings = new WebPageDownloaderSettings();

            configuration.GetSection("Application").Bind(settings);

            configuration.GetSection("Binding").Bind(settings);

            configuration.GetSection("ConnectionSettings").Bind(settings);

            await Task.Run(() => Console.WriteLine("Hello!"), stoppingToken);
        }
    }
}
