using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public class RmqConsumer : IRmqConsumer
    {
        private readonly IModel _amqpChannel;

        private string _queueName;

        private string _consumerTag;

        private string _exchange;

        private string _routingKey;

        private readonly EventingBasicConsumer _consumer;

        public RmqConsumer(IModel amqpChannel, string queueName, string exchange, string routingKey) 
        {
            _amqpChannel = amqpChannel;

            _queueName = queueName;

            _exchange = exchange;

            _routingKey = routingKey;

            _consumer = new EventingBasicConsumer(_amqpChannel);
        }

        public void Ack(BasicDeliverEventArgs args) => _amqpChannel.BasicAck(args.DeliveryTag, false);

        public void Dispose() 
        {
            _amqpChannel?.Close();

            _amqpChannel.Dispose();
        } 

        public void SetQueue(string queueName) => _queueName = queueName;

        public void SetExchange(string exchangeName) => _exchange = exchangeName;

        public void SetRoutingKey(string routingKey) => _routingKey = routingKey;

        public IBasicConsumer GetBasicConsumer() => _consumer;

        public void StartListening(EventHandler<BasicDeliverEventArgs> onReceiveHandler)
        {
            _amqpChannel.QueueBind(queue: _queueName,
                                   exchange: _exchange,
                                   routingKey: _routingKey);

            Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

            _consumer.Received += onReceiveHandler;

            _consumerTag = _amqpChannel.BasicConsume(queue: _queueName,
                                                     autoAck: false, 
                                                     consumer: _consumer);
        }

        public void StopListening()
        {
            _amqpChannel.BasicCancel(_consumerTag);

            Dispose();
        }
    }
}
