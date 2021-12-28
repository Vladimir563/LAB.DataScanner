using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.ConfigDatabaseApi.Contracts.MessageBroker;
using NSubstitute;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace LAB.DataScanner.Components.Tests.Unit.Services.MessageBroker
{
    public class RmqConsumerTest
    {
        #region Variables
        EventHandler<BasicDeliverEventArgs> onReceiveHandler;

        IEventingBasicConsumer basicConsumer;

        IRmqConsumer consumer;

        BasicDeliverEventArgs args;

        IModel channel;
        #endregion

        [SetUp]
        public void Setup()
        {
            channel = Substitute.For<IModel>();

            args = Substitute.For<BasicDeliverEventArgs>();

            basicConsumer = Substitute.For<IEventingBasicConsumer>();

            consumer = Substitute.For<RmqConsumer>(channel, "");

            onReceiveHandler = (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);

                var routingKey = ea.RoutingKey;

                consumer.Ack(ea);
            };
        }

        [Test]
        public void ShouldCall_AckMessage_OnceTheyArrivedAndHandled() 
        {
            //Act
            basicConsumer.Received += onReceiveHandler;

            basicConsumer.Received += Raise.EventWith(this, args);

            //Assert
            consumer.Received().Ack(args);
        }

        [Test]
        public void ShouldCall_BasicConsume_OnceStartListening() 
        {
            //Act
            consumer.StartListening(onReceiveHandler);

            //Assert
            channel.Received().BasicConsume("", false, consumer.GetBasicConsumer());
        }

        [Test]
        public void ShouldCall_BasicCancel_OnceStopListening() 
        {
            //Act
            consumer.StopListening();

            //Assert
            channel.Received().BasicCancel(null);
        }
    }
}
