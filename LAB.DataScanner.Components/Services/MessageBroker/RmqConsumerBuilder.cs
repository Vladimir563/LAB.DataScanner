using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public class RmqConsumerBuilder: RmqBaseBuilder<IRmqConsumer>
    {
        private IModel _channel;

        private IConnection _connection;

        private readonly IConfigurationRoot _configuration;

        private ILogger<RmqBaseBuilder<IRmqConsumer>> _logger;

        private string _queueName;

        private string _exchange;

        private string _routingKey;

        private bool _isQueueAutoCreation = false;

        public RmqConsumerBuilder(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public RmqConsumerBuilder UsingQueue(string queueName) 
        {
            _queueName = queueName;

            return this;
        }

        //TODO: why here we see the IConfigurationSection?          ????? 
        public RmqConsumerBuilder UsingConfigQueueName(IConfigurationSection bindingSection)
        {
            _queueName = bindingSection.GetSection("Binding:ReceiverQueue").Value;

            return this;
        }

        public RmqConsumerBuilder WithQueueAutoCreation()
        {
            _isQueueAutoCreation = true;

            return this;
        }

        private void PrepareConsumerConnection() 
        {
            _logger.LogInformation("PrepareConsumerConnection() method has been executed.");

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
            catch (BrokerUnreachableException e)
            {
                _logger?.LogError($"RmqConsumerBuilder: Connection to rabbitmq server has been failed [{e.Message}]");
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

            //TODO: Use the Bind (Microsoft.Extentions.Configuration.*)
            _exchange = _configuration.GetSection("Binding:ReceiverExchange").Value ?? "";

            _routingKey = _configuration.GetSection("Binding:ReceiverRoutingKey").Value ?? "";

            #endregion

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_exchange, "topic");

            _channel.QueueDeclare(_queueName);

            _channel.QueueBind(queue: _queueName,
            exchange: _exchange,
            routingKey: _routingKey);

            return new RmqConsumer(_channel, _queueName);
        }

        public override RmqBaseBuilder<IRmqConsumer> UsingLogger(ILogger<RmqBaseBuilder<IRmqConsumer>> logger)
        {
            _logger = logger;

            return this;
        }
    }
}
