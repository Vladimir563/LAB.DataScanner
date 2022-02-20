using LAB.DataScanner.Components.Services.Generators;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Settings;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using System.IO;
using System.Text;

namespace LAB.DataScanner.Components.Tests.Unit.Services.Generators
{
    public class UrlsGeneratorEngineTests
    {
        private IRmqPublisher _rmqPublisherServiceMock;

        [SetUp]
        public void Setup()
        {
            _rmqPublisherServiceMock = Substitute.For<IRmqPublisher>();
        }

        [Test]
        public void ShouldGenerateAndPublishUrlsBasedOnConfiguration()
        {
            //Arrange
            var urlsGenerator = ConfigInitialize("generateAndPublishTestConfig.json");

            //Act
            urlsGenerator.Start();

            //Assert
            _rmqPublisherServiceMock
                .Received(1)
                .Publish(Arg.Is<byte[]>(e => Encoding.UTF8.GetString(e) == "http://testSite/0/3/4"),
                Arg.Is("TargetExchange"),
                Arg.Is<string[]>(a => a[0].Equals("A")));

        }

        [Test]
        public void ShouldSkipPublishingIfNoAnyBindingsInfo()
        {
            //Arrange
            var urlsGenerator = ConfigInitialize("skipPublishingTestConfig.json");

            //Act
            urlsGenerator.Start();

            //Assert
            _rmqPublisherServiceMock
            .Received(0)
            .Publish(Arg.Is<byte[]>(e => Encoding.UTF8.GetString(e) == "http://testSite/0/3/4"),
            Arg.Is("TargetExchange"),
            Arg.Is<string[]>(a => a[0].Equals("A")));
        }

        private UrlsGeneratorEngine ConfigInitialize(string configName) 
        {
            Directory.SetCurrentDirectory(@"../../../Unit/Services/Generators/");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configName)
                .Build();

            var rmqPublisherSettings = new RmqPublisherSettings();

            configuration.GetSection("PublisherSettings").Bind(rmqPublisherSettings);

            var urlsGeneratorSettings = new UrlsGeneratorSettings();

            configuration.GetSection("Application").Bind(urlsGeneratorSettings);

            return Substitute.For<UrlsGeneratorEngine>(_rmqPublisherServiceMock,
                urlsGeneratorSettings, rmqPublisherSettings);
        }
    }
}
