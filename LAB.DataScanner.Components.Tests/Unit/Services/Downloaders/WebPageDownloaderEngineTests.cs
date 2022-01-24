using LAB.DataScanner.Components.Services.Downloaders;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Text;

namespace LAB.DataScanner.Components.Tests.Unit.Services.Downloaders
{
    public class WebPageDownloaderEngineTests
    {
        private BasicDeliverEventArgs _args;

        private IConfigurationRoot _fakeConfig;

        private IModel _amqpChannelMock;

        private IRmqConsumer _rmqConsumerMock;

        private IRmqPublisher _rmqPublisherMock;

        private WebPageDownloaderEngine _webPageDownloaderEngine;

        private IDataRetriever _dataRetriever;

        [SetUp]
        public void Setup()
        {
            _args = Substitute.For<BasicDeliverEventArgs>();

            var configDic = new Dictionary<string, string>
            {
                {"Binding:ReceiverQueue", "HtmlToJsonConverterQueue"},
                {"Binding:ReceiverExchange", "WebPageDownloaderExchange"},
                {"Binding:ReceiverRoutingKey", "#"},
                {"Binding:SenderExchange", "HtmlToJsonConverterExchange"},
                {"Binding:SenderRoutingKeys", "['#']"}
            };

            var builder = new ConfigurationBuilder().AddInMemoryCollection(configDic);

            _fakeConfig = builder.Build();

            _amqpChannelMock = Substitute.For<IModel>();

            _rmqConsumerMock = Substitute.For<RmqConsumer>
                (_amqpChannelMock, null);

            _rmqPublisherMock = Substitute.For<IRmqPublisher>();

            _dataRetriever = Substitute.For<IDataRetriever>();

            _webPageDownloaderEngine = Substitute.For<WebPageDownloaderEngine>
            (_fakeConfig.GetSection("Binding"),
            _dataRetriever,
            _rmqPublisherMock,
            _rmqConsumerMock,
            Substitute.For<UrlsValidator>());
        }

        [Test]
        public void ShouldSkipNotValidLink() 
        {
            //Assign
            var _invalidUrl = Encoding.UTF8.GetBytes("not_valid_url");

            _args.Body = _invalidUrl;

            //Act
            _webPageDownloaderEngine.OnReceive(this, _args);

            //Assert
            _rmqPublisherMock.DidNotReceive().Publish(new byte[] { }, "", new string[] { });

        }

        [Test]
        public void ShouldHandleValidLink() 
        {
            //Assign
            var _validUrl = Encoding.UTF8.GetBytes("https://www.epam.com/careers/job-listings?query=1&country=Russia");

            _args.Body = _validUrl;

            //Act
            _webPageDownloaderEngine.OnReceive(this, _args);

            //Assert
            _rmqPublisherMock.Received(1).Publish(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<string[]>());
        }

        [Test]
        public void ShouldPublishToExchangePageAsIsOnceSucessfullDownload() 
        { 
        
        }

        [Test]
        public void ShouldAcknowledgeMessageOncePageDownloadingBeenComplete() { }
    }
}
