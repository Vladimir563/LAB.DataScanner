using LAB.DataScanner.Components.Interfaces.Downloaders;
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

namespace LAB.DataScanner.Components.Tests.Unit.Services.MessageBroker
{
    public class RmqConsumerWebPageDownloaderTests
    {
        private byte[] _bodyUrlParameter;

        private BasicDeliverEventArgs _args;

        private ILogger _logger;

        [SetUp]
        public void Setup() 
        {
            _bodyUrlParameter = Encoding.UTF8.GetBytes("https://test1/test2");

            _args = Substitute.For<BasicDeliverEventArgs>();

            _args.Body = _bodyUrlParameter;

            _logger = Substitute.For<ILogger<WebPageDownloaderEngine>>();
        }

        [Test]
        public void ShouldCall_AckMessage_OnceTheyArrivedAndHandled() 
        {
            //Arrange
            IRmqConsumer _rmqConsumerMock = Substitute.For<IRmqConsumer>();

            //WebPageDownloaderEngine _webPageDownloaderEngineMock = Substitute.For<WebPageDownloaderEngine>
            //(Substitute.For<IConfigurationRoot>(),
            //Substitute.For<IDataRetriever>(),
            //Substitute.For<IRmqPublisher>(),
            //_rmqConsumerMock,
            //Substitute.For<IUrlsValidator>(),
            //_logger);

            //Act
            //_webPageDownloaderEngineMock.OnReceive(this, _args);

            //Assert
            _rmqConsumerMock.Received(1).Ack(_args);
        }

        [Test]
        public void ShouldCall_BasicConsume_OnceStartListening() 
        {
            //Arrange
            IModel _amqpChannelMock = Substitute.For<IModel>();

            IRmqConsumer _rmqConsumerMock = Substitute.For<RmqConsumer>
                (_amqpChannelMock, null);

            //WebPageDownloaderEngine _webPageDownloaderEngineMock = Substitute.For<WebPageDownloaderEngine>
            //(Substitute.For<IConfigurationRoot>(),
            //Substitute.For<IDataRetriever>(),
            //Substitute.For<IRmqPublisher>(),
            //_rmqConsumerMock,
            //Substitute.For<IUrlsValidator>(),
            //_logger);

            //Act
            //_rmqConsumerMock.StartListening(_webPageDownloaderEngineMock.OnReceive);

            //Assert
            _amqpChannelMock.Received(1).BasicConsume(null, false, _rmqConsumerMock.GetBasicConsumer());
        }

        [Test]
        public void ShouldCall_BasicCancel_OnceStopListening() 
        {
            //Arrange
            IModel _amqpChannelMock = Substitute.For<IModel>();

            IRmqConsumer _rmqConsumerMock = Substitute.For<RmqConsumer>
                (_amqpChannelMock, null);

            //Act
            _rmqConsumerMock.StopListening();

            //Assert
            _amqpChannelMock.Received(1).BasicCancel(null);
        }
    }
}
