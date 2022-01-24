using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public class RmqPublisherBuilder : RmqBaseBuilder<IRmqPublisher>
    {
        private IModel _channel;

        private IConnection _connection;

        private string _exchange;

        private string _routingKey = "#";

        private bool _isExchangeAutoCreation = false;

        public RmqPublisherBuilder UsingExchange(string exchange) 
        {
            _exchange = exchange;

            return this;
        }   

        public RmqPublisherBuilder UsingRoutingKey(string routingKey) 
        {
            _routingKey = routingKey;

            return this;
        }

        public RmqPublisherBuilder UsingExchangeAndRoutingKey(string exchange, string routingKey)
        {
            _exchange = exchange;

            _routingKey = routingKey;

            return this;
        }

        public RmqPublisherBuilder UsingConfigExchangeAndRoutingKey(IConfigurationSection configurationSection)
        {
            _exchange = configurationSection.GetSection("SenderExchange").Value;

            _routingKey = configurationSection.GetSection("SenderRoutingKey").Value;

            return this;
        }

        public RmqPublisherBuilder WithExchangeAutoCreation()
        {
            _isExchangeAutoCreation = true;

            return this;
        }

        private void PreparePublisherConnection() 
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

        public override IRmqPublisher Build()
        {
            PreparePublisherConnection();

            _channel = _connection.CreateModel();

            if (_exchange is null || _isExchangeAutoCreation) 
            {
                _exchange = "exchange_" + Guid.NewGuid();
            }

            _channel.ExchangeDeclare(_exchange, "topic");

            return new RmqPublisher(_channel, _exchange, _routingKey);
        }
    }
}
