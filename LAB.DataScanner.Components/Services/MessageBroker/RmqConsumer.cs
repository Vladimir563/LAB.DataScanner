using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public class RmqConsumer : IRmqConsumer
    {
        private readonly IModel _amqpChannel;

        private string _queueName;

        private string _consumerTag;

        private readonly EventingBasicConsumer _consumer;

        public RmqConsumer(IModel amqpChannel, string queueName) 
        {
            _amqpChannel = amqpChannel;

            _queueName = queueName;

            _consumer = new EventingBasicConsumer(_amqpChannel);
        }

        public void Ack(BasicDeliverEventArgs args) => _amqpChannel.BasicAck(args.DeliveryTag, false);

        public void Dispose() 
        {
            _amqpChannel?.Close();

            _amqpChannel?.Dispose();
        } 

        public void SetQueue(string queueName) => _queueName = queueName;

        public IBasicConsumer GetBasicConsumer() => _consumer;

        public void StartListening(EventHandler<BasicDeliverEventArgs> onReceiveHandler)
        {
            //TODO: where is a logger
            Debug.Print(" [*] Waiting for messages. To exit press CTRL+C");

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
