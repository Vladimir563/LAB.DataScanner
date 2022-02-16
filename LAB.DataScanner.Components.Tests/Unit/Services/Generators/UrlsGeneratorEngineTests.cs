using LAB.DataScanner.Components.Services.Generators;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace LAB.DataScanner.Components.Tests.Unit.Services.Generators
{
    public class UrlsGeneratorEngineTests
    {
        private IRmqPublisher _rmqPublisherServiceMock;

        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _rmqPublisherServiceMock = Substitute.For<IRmqPublisher>();

            _logger = Substitute.For<ILogger<UrlsGeneratorEngine>>();
        }

        [Test]
        public void ShouldGenerateAndPublishUrlsBasedOnConfiguration()
        {
            //TODO: use the config for the project
            //Arrange
            var configDic = new Dictionary<string, string>
            {
                { "Application:UrlTemplate", "http://testSite/{0}/{1}/{2}" },
                { "Application:Sequences", "['0..2', '3..6', '4..5']" },

                { "Binding:SenderExchange", "TargetExchange" },
                { "Binding:SenderRoutingKeys", "['A', 'B']" }
            };

            var builder = new ConfigurationBuilder().AddInMemoryCollection(configDic);

            var fakeConfiguration = builder.Build();

            var urlsGenerator = Substitute.For<UrlsGeneratorEngine>(_rmqPublisherServiceMock,
                fakeConfiguration, _logger);

            //Act
            urlsGenerator.Generate();

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
            var configDic = new Dictionary<string, string>
            {
                { "Application:UrlTemplate", "http://testSite/{0}/{1}/{2}" },
                { "Application:Sequences", "['0..2', '3..6', '4..5']" },

                { "Binding:TargetExchange", "" },
                { "Binding:RoutingKeys", "" }
            };

            var builder = new ConfigurationBuilder()
            .AddInMemoryCollection(configDic);

            var fakeConfigurationSection = builder.Build();

            var urlsGenerator = Substitute.For<UrlsGeneratorEngine>(_rmqPublisherServiceMock,
                fakeConfigurationSection, _logger);

            //Act
            urlsGenerator.Generate();

            //Assert
            _rmqPublisherServiceMock
            .Received(0)
            .Publish(Arg.Is<byte[]>(e => Encoding.UTF8.GetString(e) == "http://testSite/0/3/4"),
            Arg.Is("TargetExchange"),
            Arg.Is<string[]>(a => a[0].Equals("A")));
        }
    }
}
