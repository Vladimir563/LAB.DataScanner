using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace LAB.DataScanner.Components.Services.MessageBroker.Interfaces
{
    public interface IRmqConsumer : IDisposable
    {
        void Ack(BasicDeliverEventArgs args);
        public void StartListening(EventHandler<BasicDeliverEventArgs> onReceiveHandler);
        public void StopListening();
        public void SetQueue(string queueName);
        public IBasicConsumer GetBasicConsumer();
    }
}
