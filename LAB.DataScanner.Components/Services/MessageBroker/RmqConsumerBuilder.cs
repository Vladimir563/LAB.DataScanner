using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Settings;
using RabbitMQ.Client;
using System;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public class RmqConsumerBuilder: RmqBaseBuilder<IRmqConsumer>
    {
        private IModel _channel;

        private IConnection _connection;

        private readonly RmqConsumerSettings _consumerSettings;

        private string _queueName;

        private string _exchange;

        private string _routingKey;

        private string _exchangeType;

        private bool _isQueueAutoCreation = false;

        public RmqConsumerBuilder(RmqConsumerSettings consumerSettings)
        {
            _consumerSettings = consumerSettings;
        }

        public RmqConsumerBuilder UsingQueue(string queueName) 
        {
            _queueName = queueName;

            return this;
        }

        public RmqConsumerBuilder UsingConfigQueueName(RmqConsumerSettings consumerSettings)
        {
            _queueName = consumerSettings.ReceiverQueue;

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
            catch
            {
                throw;
            }
        }

        public override IRmqConsumer Build()
        {
            PrepareConsumerConnection();

            #region Configuration queue parameters

            if (_queueName is null || _isQueueAutoCreation)
            {
                _queueName = "queue_" + Guid.NewGuid();
            }

            _exchange = _consumerSettings.ReceiverExchange;

            _routingKey = _consumerSettings.ReceiverRoutingKey;

            _exchangeType = _consumerSettings.ExchangeType;

            #endregion

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_exchange, _exchangeType);

            _channel.QueueDeclare(_queueName);

            _channel.QueueBind(queue: _queueName,
                                exchange: _exchange,
                                routingKey: _routingKey);

            return new RmqConsumer(_channel, _queueName);
        }
    }
}
