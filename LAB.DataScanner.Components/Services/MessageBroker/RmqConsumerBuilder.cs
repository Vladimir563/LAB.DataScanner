using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public class RmqConsumerBuilder: RmqBaseBuilder<IRmqConsumer>
    {
        private IModel _channel;

        private IConnection _connection;

        private readonly IConfigurationSection _bindingSection;

        private string _queueName;

        private string _exchange;

        private string _routingKey;

        private bool _isQueueAutoCreation = false;

        public RmqConsumerBuilder(IConfigurationSection bindingSection)
        {
            _bindingSection = bindingSection;
        }

        public RmqConsumerBuilder UsingQueue(string queueName) 
        {
            _queueName = queueName;

            return this;
        }

        public RmqConsumerBuilder UsingConfigQueueName(IConfigurationSection bindingSection)
        {
            _queueName = bindingSection.GetSection("ReceiverQueue").Value;

            return this;
        }

        public RmqConsumerBuilder WithQueueAutoCreation()
        {
            _isQueueAutoCreation = true;

            return this;
        }

        private void PrepareConsumerConnection() 
        {
            var connectionFactory = new ConnectionFactory
            {
                UserName = this.UserName,
                Password = this.Password,
                HostName = this.HostName,
                Port = this.Port,
                VirtualHost = this.VirtualHost
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

            #region Configuration queue parameters

            if (_isQueueAutoCreation || _queueName is null) 
            {
                _queueName = "queue_" + Guid.NewGuid();
            }

            _exchange = _bindingSection.GetSection("ReceiverExchange").Value ?? "";

            _routingKey = _bindingSection.GetSection("ReceiverRoutingKey").Value ?? "";

            #endregion

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_exchange, "topic");

            _channel.QueueDeclare(_queueName);

            _channel.QueueBind(queue: _queueName,
            exchange: _exchange,
            routingKey: _routingKey);

            return new RmqConsumer(_channel, _queueName);
        }
    }
}
