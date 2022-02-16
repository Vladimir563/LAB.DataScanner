﻿using LAB.DataScanner.Components.Interfaces.Downloaders;
using LAB.DataScanner.Components.Services.Downloaders;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace LAB.DataScanner.Components.Tests.Unit.Services.Downloaders
{
    public class WebPageDownloaderEngineTests
    {
        private BasicDeliverEventArgs _args;

        private IModel _amqpChannelMock;

        private IRmqConsumer _rmqConsumerMock;

        private IRmqPublisher _rmqPublisherMock;

        private IDataRetriever _dataRetrieverMock;

        private IUrlsValidator _urlsValidatorMock;

        private WebPageDownloaderEngine _webPageDownloaderEngine;

        [SetUp]
        public void Setup()
        {
            //Arrange
            _args = Substitute.For<BasicDeliverEventArgs>();

            _amqpChannelMock = Substitute.For<IModel>();

            _rmqConsumerMock = Substitute.For<RmqConsumer>
                (_amqpChannelMock, null);

            _rmqPublisherMock = Substitute.For<IRmqPublisher>();

            _dataRetrieverMock = Substitute.For<IDataRetriever>();

            //TODO: There are not any reasons to have a class for such needs like you implemented in the UrlsValidator. Use Helpers
            _urlsValidatorMock = Substitute.For<UrlsValidator>(Substitute.For<ILogger<UrlsValidator>>());

            _webPageDownloaderEngine = Substitute.For<WebPageDownloaderEngine>
            (Substitute.For<IConfigurationRoot>(),
            _dataRetrieverMock,
            _rmqPublisherMock,
            _rmqConsumerMock,
            _urlsValidatorMock,
            Substitute.For<ILogger<WebPageDownloaderEngine>>());

            var _validUrl = Encoding.UTF8.GetBytes("https://www.epam.com/careers/job-listings?query=1&country=Russia");

            _args.Body = _validUrl;

        }

        [Test]
        public void ShouldSkipNotValidLink() 
        {
            //TODO: To my mind, the flow should be:
            //TODO: 1. start listening for the _webPageDownloaderEngine
            //TODO: 2. publish the message for the _webPageDownloaderEngine
            //TODO: 3. now you can check _dataRetrieverMock.DidNotReceive().RetrieveStringAsync for instance 

            //Arrange
            var _invalidUrl = Encoding.UTF8.GetBytes("not_valid_url");

            _args.Body = _invalidUrl;

            //Act
            _webPageDownloaderEngine.OnReceive(this, _args);

            //Assert
            _dataRetrieverMock.DidNotReceive().RetrieveStringAsync(Arg.Any<string>());
        }

        [Test]
        public void ShouldHandleValidLink() 
        {
            //TODO: see the previous method
            //Act
            _webPageDownloaderEngine.OnReceive(this, _args);

            //Assert
            _dataRetrieverMock.Received(1).RetrieveStringAsync(Arg.Any<string>());
        }

        [Test]
        public void ShouldPublishToExchangePageAsIsOnceSucessfullDownload() 
        {
            //TODO: see the previous method
            //Act
            _webPageDownloaderEngine.OnReceive(this, _args);

            //Assert
            _rmqPublisherMock.Received(1).Publish(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<string[]>());
        }

        [Test]
        public void ShouldAcknowledgeMessageOncePageDownloadingBeenComplete() 
        {
            //TODO: see the previous method
            //Act
            _webPageDownloaderEngine.OnReceive(this, _args);

            //Assert
            //TODO: Why is the parameter 0?
            _rmqConsumerMock.Received(0).Ack(_args);
        }
    }
}
