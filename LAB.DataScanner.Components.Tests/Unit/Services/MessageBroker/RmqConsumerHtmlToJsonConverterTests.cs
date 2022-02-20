using LAB.DataScanner.Components.Interfaces.Converters;
using LAB.DataScanner.Components.Services.Converters;
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
    public class RmqConsumerHtmlToJsonConverterTests
    {
        private byte[] _fakeHtmlContent;

        private BasicDeliverEventArgs _args;

        private IConfigurationRoot _fakeConfig;

        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _fakeHtmlContent = Encoding.UTF8.GetBytes("<li class=\"search-result__item\"></li> <li class=\"search-result__item\"></li>");

            _args = Substitute.For<BasicDeliverEventArgs>();

            _args.Body = _fakeHtmlContent;

            _logger = Substitute.For<ILogger<HtmlToJsonConverterEngine>>();

            _fakeConfig = Substitute.For<IConfigurationRoot>();

        }
        [Test]
        public void ShouldCall_AckMessage_OnceTheyArrivedAndHandled()
        {
            //Arrange
            IRmqConsumer _rmqConsumerMock = Substitute.For<IRmqConsumer>();

            HtmlToJsonConverterEngine _htmlToJsonConverterEngine = Substitute.For<HtmlToJsonConverterEngine>
            (_fakeConfig,
            Substitute.For<IRmqPublisher>(),
            _rmqConsumerMock,
            _logger);

            //Act
            //_htmlToJsonConverterEngine.OnReceive(this, _args);

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

            HtmlToJsonConverterEngine _htmlToJsonConverterEngine = Substitute.For<HtmlToJsonConverterEngine>
            (_fakeConfig,
            Substitute.For<IRmqPublisher>(),
            _rmqConsumerMock,
            _logger);

            //Act
            //_rmqConsumerMock.StartListening(_htmlToJsonConverterEngine.OnReceive);

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
