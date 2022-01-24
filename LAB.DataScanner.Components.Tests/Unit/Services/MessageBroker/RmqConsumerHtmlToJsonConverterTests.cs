using LAB.DataScanner.Components.Services.Converters;
using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Text;

namespace LAB.DataScanner.Components.Tests.Unit.Services.MessageBroker
{
    public class RmqConsumerHtmlToJsonConverterTests
    {
        private byte[] _fakeHtmlContent;

        private BasicDeliverEventArgs _args;

        private IConfigurationRoot _fakeConfig;

        [SetUp]
        public void Setup()
        {
            _fakeHtmlContent = Encoding.UTF8.GetBytes("<li class=\"search-result__item\"></li> <li class=\"search-result__item\"></li>");

            _args = Substitute.For<BasicDeliverEventArgs>();

            _args.Body = _fakeHtmlContent;

            var configDic = new Dictionary<string, string>
            {
                {"Application:HtmlDataDownloadingMethod", "dynamic"},
                {"Application:WebBrowser", "chrome"},
                {"Application:HtmlFragmentStrategy", "selectNodes"},
                {"Application:HtmlFragmentExpression", "//*[@class='search-result__item']"},

                {"Binding:ReceiverQueue", "HtmlToJsonConverterQueue"},
                {"Binding:ReceiverExchange", "WebPageDownloaderExchange"},
                {"Binding:ReceiverRoutingKey", "#"},
                {"Binding:SenderExchange", "HtmlToJsonConverterExchange"},
                {"Binding:SenderRoutingKeys", "['#']"},

                {"HtmlDataDownloadingSettingsArrs:HtmlDataDownloadingMethods","['dynamic','static']"},
                {"HtmlDataDownloadingSettingsArrs:WebBrowsers","['chrome','fireFox','mozilla','microsoftEdge','opera']"},
                {"HtmlDataDownloadingSettingsArrs:HtmlFragmentStrategies","['selectNodes','selectSingleNode']"}
            };

            var builder = new ConfigurationBuilder().AddInMemoryCollection(configDic);

            _fakeConfig = builder.Build();

        }
        [Test]
        public void ShouldCall_AckMessage_OnceTheyArrivedAndHandled()
        {
            //Assign
            IRmqConsumer _rmqConsumerMock = Substitute.For<IRmqConsumer>();

            HtmlToJsonConverterEngine _htmlToJsonConverterEngine = Substitute.For<HtmlToJsonConverterEngine>
            (_fakeConfig,
            Substitute.For<IRmqPublisher>(),
            _rmqConsumerMock);

            //Act
            _htmlToJsonConverterEngine.OnReceive(this, _args);

            //Assert
            _rmqConsumerMock.Received(1).Ack(_args);
        }

        [Test]
        public void ShouldCall_BasicConsume_OnceStartListening()
        {
            //Assign
            IModel _amqpChannelMock = Substitute.For<IModel>();

            IRmqConsumer _rmqConsumerMock = Substitute.For<RmqConsumer>
                (_amqpChannelMock, null);

            HtmlToJsonConverterEngine _htmlToJsonConverterEngine = Substitute.For<HtmlToJsonConverterEngine>
            (_fakeConfig,
            Substitute.For<IRmqPublisher>(),
            _rmqConsumerMock);

            //Act
            _rmqConsumerMock.StartListening(_htmlToJsonConverterEngine.OnReceive);

            //Assert
            _amqpChannelMock.Received(1).BasicConsume(null, false, _rmqConsumerMock.GetBasicConsumer());
        }

        [Test]
        public void ShouldCall_BasicCancel_OnceStopListening()
        {
            //Assign
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
