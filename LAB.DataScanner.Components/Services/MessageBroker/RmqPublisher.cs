using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using RabbitMQ.Client;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public class RmqPublisher : IRmqPublisher
    {
        private readonly IModel _amqpChannel;

        private string _exchange;

        private string _routingKey;

        public RmqPublisher(IModel amqpChannel, string exchange, string routingKey) 
        {
            _amqpChannel = amqpChannel; 
            
            _exchange = exchange;

            _routingKey = routingKey;
        }
        public void Dispose()
        {
            _amqpChannel?.Close();

            _amqpChannel.Dispose();
        }

        public void SetExchange(string exchange) => _exchange = exchange;

        public void SetRoutingKey(string routingKey) => _routingKey = routingKey;

        public void Publish(byte[] message)
        {
            _amqpChannel.BasicPublish(_exchange,
                                      _routingKey,
                                      basicProperties: null,
                                      message);
        }

        public void Publish(byte[] message, string routingKey)
        {
            _amqpChannel.BasicPublish(_exchange,
                                      routingKey,
                                      basicProperties: null,
                                      message);
        }

        public void Publish(byte[] message, string exchange, string routingKey)
        {
            _amqpChannel.BasicPublish(exchange,
                                      routingKey,
                                      basicProperties: null,
                                      message);
        }

        public void Publish(byte[] outputData, string exchangeName, string[] routingKeys)
        {
            foreach (var routingKey in routingKeys)
            {
                _amqpChannel.BasicPublish(exchangeName,
                                          routingKey,
                                          basicProperties: null,
                                          outputData);
            }
        }
    }
}
