using LAB.DataScanner.Components.Services.Generators;
using LAB.DataScanner.ConfigDatabaseApi.Contracts.MessageBroker;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace LAB.DataScanner.Components.Tests.Unit.Services.Generators
{
    public class UrlsGeneratorEngineTest
    {
        IRmqPublisher rmqPublisherServiceMock;

        [SetUp]
        public void Setup()
        {
            rmqPublisherServiceMock = Substitute.For<IRmqPublisher>();
        }

        [Test]
        public void ShouldGenerateAndPublishUrlsBasedOnConfiguration()
        {
            //Arrange
            var configDic = new Dictionary<string, string>
            {
                { "Application:UrlTemplate", "http://testSite/{0}/{1}/{2}" },
                { "Application:Sequences", "['0..2', '3..6', '4..5']" },

                { "Binding:SenderExchange", "TargetExchange" },
                { "Binding:SenderRoutingKeys", "['A', 'B']" }
            };

            var builder = new ConfigurationBuilder().AddInMemoryCollection(configDic);

            var fakeConfigurationSection = builder.Build();

            var urlsGenerator = Substitute.For<UrlsGeneratorEngine>(rmqPublisherServiceMock,
                fakeConfigurationSection.GetSection("Application"),
                fakeConfigurationSection.GetSection("Binding"));

            //Act
            urlsGenerator.Start();

            //Assert
            rmqPublisherServiceMock
            .Received()
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

            var urlsGenerator = Substitute.For<UrlsGeneratorEngine>(rmqPublisherServiceMock,
                fakeConfigurationSection.GetSection("Application"),
                fakeConfigurationSection.GetSection("Binding"));

            //Act
            urlsGenerator.Start();

            //Assert
            rmqPublisherServiceMock
            .Received(0)
            .Publish(Arg.Is<byte[]>(e => Encoding.UTF8.GetString(e) == "http://testSite/0/3/4"),
            Arg.Is("TargetExchange"),
            Arg.Is<string[]>(a => a[0].Equals("A")));
        }
    }
}
