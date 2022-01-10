using LAB.DataScanner.Components.Services.Downloaders;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Tests.Unit.Services.MessageBroker;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace LAB.DataScanner.Components.Tests.Unit.Services.Downloaders
{
    public class WebPageDownloaderEngineTests
    {
        WebPageDownloaderEngine engineMock;

        IRmqConsumer rmqConsumerMock;

        IRmqPublisher rmqPublisherMock;

        IDataRetriever dataRetrieverMock;

        IConfigurationSection bindingSectionMock;

        IEventingBasicConsumer basicConsumer;

        BasicDeliverEventArgs args;

        [SetUp]
        public void Setup()
        {
            basicConsumer = Substitute.For<IEventingBasicConsumer>();

            args = Substitute.For<BasicDeliverEventArgs>();

            bindingSectionMock = Substitute.For<IConfigurationSection>();

            rmqConsumerMock = Substitute.For<IRmqConsumer>();

            rmqPublisherMock = Substitute.For<IRmqPublisher>();

            dataRetrieverMock = Substitute.For<IDataRetriever>();

            engineMock = Substitute.For<WebPageDownloaderEngine>(bindingSectionMock, dataRetrieverMock, rmqPublisherMock, rmqConsumerMock);
        }

        [Test]
        public void ShouldSkipNotValidLink() 
        {
            //Assign
            var notValidUrl = "not_valid_url";

            //Act
            engineMock.Start();

            //Assert

        }

        [Test]
        public void ShouldHandleValidLink() { }

        [Test]
        public void ShouldPublishToExchangePageAsIsOnceSucessfullDownload() { }

        [Test]
        public void ShouldAcknowledgeMessageOncePageDownloadingBeenComplete() { }
    }
}
