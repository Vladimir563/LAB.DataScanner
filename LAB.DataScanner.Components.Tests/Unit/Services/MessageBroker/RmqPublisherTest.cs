using LAB.DataScanner.Components.Services.MessageBroker;
using LAB.DataScanner.ConfigDatabaseApi.Contracts.MessageBroker;
using NSubstitute;
using NUnit.Framework;

namespace LAB.DataScanner.Components.Tests.Unit.Services.MessageBroker
{
    public class RmqPublisherTest
    {
        IRmqPublisher publisher;

        RmqPublisherBuilder publisherBuilder;

        byte[] message;

        [SetUp]
        public void Setup()
        {
            //Assign
            publisherBuilder = Substitute.For<RmqPublisherBuilder>();

            message = new byte[] { };
        }

        [Test]
        public void ShouldPublishMessageToDefaultExchange() 
        {
            //Act
            publisher = publisherBuilder.Build();

            publisher.Publish(message);

            //Assert
            publisher.Received().Publish(message);
            
        }

        [Test]
        public void ShouldPublishMessageWithRoutingKey() 
        {
            //Act
            publisher = publisherBuilder.UsingRoutingKey("").Build();

            publisher.Publish(message);

            //Assert
            publisher.Received().Publish(message);
        }

        [Test]
        public void ShouldPublishMessageToCertainExchangeAndRoutingKey() 
        {
            //Act
            publisher = publisherBuilder.UsingExchangeAndRoutingKey("","").Build();

            publisher.Publish(message);

            //Assert
            publisher.Received().Publish(message);
        }
    }
}
