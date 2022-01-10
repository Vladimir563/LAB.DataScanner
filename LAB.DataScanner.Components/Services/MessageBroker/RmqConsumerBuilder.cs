using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public class RmqConsumerBuilder: RmqBaseBuilder<IRmqConsumer>
    {
        private IModel _channel;

        private IConnection _connection;

        private string _queueName;

        private string _exchange;

        private string _routingKey;

        public RmqConsumerBuilder UsingQueue(string queueName) 
        {
            _queueName = queueName;

            return this;
        }

        public RmqConsumerBuilder UsingConfigQueueName(IConfigurationSection configurationSection)
        {
            _queueName = configurationSection.GetSection("ReceiverQueue").Value;

            return this;
        }

        public RmqConsumerBuilder UsingConfigSettings(IConfigurationSection configurationSection)
        {
            _queueName = configurationSection.GetSection("ReceiverQueue").Value;

            _exchange = configurationSection.GetSection("ReceiverExchange").Value;

            _routingKey = JsonConvert.DeserializeObject<string[]>(configurationSection.GetSection("ReceiverRoutingKeys").Value ?? "")[0];

            return this;
        }

        public RmqConsumerBuilder WithQueueAutoCreation()
        {
            _queueName = "queue_" + Guid.NewGuid();

            return this;
        } //?I'm not sure is this method implemented properly

        private void PrepareConsumerConnection() 
        {
            var connectionFactory = new ConnectionFactory
            {
                UserName = this.userName,
                Password = this.password,
                HostName = this.hostName,
                Port = this.port,
                VirtualHost = this.virtualHost
            };

            try
            {
                _connection = connectionFactory.CreateConnection();
            }
            catch (BrokerUnreachableException)
            {
                throw new NullReferenceException("Connection to rabbitmq server failed.");
            }
        }

        public override IRmqConsumer Build()
        {
            PrepareConsumerConnection();

            _channel = _connection.CreateModel();

            return new RmqConsumer(_channel, _queueName, _exchange, _routingKey);
        }
    }
}
