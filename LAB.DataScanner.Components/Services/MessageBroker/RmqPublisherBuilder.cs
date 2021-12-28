using LAB.DataScanner.ConfigDatabaseApi.Contracts.MessageBroker;
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

        private string _exchange = "";

        private string _routingKey = ""; 

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
            _exchange = configurationSection.GetSection("Binding").GetSection("SenderExchange").Value;

            _routingKey = configurationSection.GetSection("Binding").GetSection("SenderRoutingKeys").Value;

            return this;
        }

        public RmqPublisherBuilder WithExchangeAutoCreation()
        {
            var autoGenExchange = "exchange_" + Guid.NewGuid();

            _exchange = autoGenExchange;

            return this;
        }

        private void PreparePublisherConnection() 
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

        public override IRmqPublisher Build()
        {
            PreparePublisherConnection();

            _channel = _connection.CreateModel();

            return new RmqPublisher(_channel, _exchange, _routingKey);
        }
    }
}
