using LAB.DataScanner.Components.Interfaces.Engines;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Services.Validators;
using LAB.DataScanner.Components.Settings;
using LAB.DataScanner.HtmlToJsonConverter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LAB.DataScanner.SimpleTableDBPersister
{
    internal class Program
    {
        static void Main()
        {
            #region Configuration, settings binding and validation
            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var dbPersisterSettings = new SimpleTableDBPersisterSettings();

            configuration.GetSection("ConnectionSettings").Bind(dbPersisterSettings);

            configuration.GetSection("DBTableCreationSettings").Bind(dbPersisterSettings);

            SettingsValidator.Validate(dbPersisterSettings);

            var rqmConsumerSettings = new RmqConsumerSettings();

            configuration.GetSection("ConsumerSettings").Bind(rqmConsumerSettings);

            SettingsValidator.Validate(rqmConsumerSettings);
            #endregion

            var rmqConsumer = new RmqConsumerBuilder(rqmConsumerSettings)
                .UsingConfigQueueName(rqmConsumerSettings)
                .UsingConfigConnectionSettings(dbPersisterSettings)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IRmqConsumer>(rmqConsumer)
                .AddSingleton<SimpleTableDBPersisterSettings>(dbPersisterSettings)
                .AddSingleton<RmqConsumerSettings>(rqmConsumerSettings)
                .AddSingleton<IEngine, DBPersisterEngine>()
                .BuildServiceProvider();

            var dbPersisterEngine = serviceProvider.GetService<IEngine>();

            dbPersisterEngine.Start();

            Console.ReadKey();
        }
    }
}