using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Settings;
using RabbitMQ.Client;
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

        private string _exchangeType;

        public RmqPublisherBuilder UsingExchange(string exchange, string exchangeType) 
        {
            _exchange = exchange;

            _exchangeType = exchangeType;

            return this;
        }   

        public RmqPublisherBuilder UsingRoutingKey(string routingKey) 
        {
            _routingKey = routingKey;

            return this;
        }

        public RmqPublisherBuilder UsingExchangeAndRoutingKey(string exchange, string exchangeType, string routingKey)
        {
            _exchange = exchange;

            _routingKey = routingKey;

            _exchangeType = exchangeType;

            return this;
        }

        public RmqPublisherBuilder UsingConfigExchangeAndRoutingKey(RmqPublisherSettings rmqPublisherSettings)
        {
            _exchange = rmqPublisherSettings.SenderExchange;

            _routingKey = rmqPublisherSettings.BasicSenderRoutingKey;

            _exchangeType = rmqPublisherSettings.ExchangeType;

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
            catch
            {
                throw;
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

            if (_exchangeType is null || _exchangeType.Equals(string.Empty)) 
            {
                _exchangeType = "topic";
            }

            _channel.ExchangeDeclare(_exchange, _exchangeType);

            return new RmqPublisher(_channel, _exchange, _routingKey);
        }
    }
}
