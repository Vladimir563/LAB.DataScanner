using System;

namespace LAB.DataScanner.Components.Services.MessageBroker.Interfaces
{
    public interface IRmqPublisher : IDisposable
    {
        void Publish(byte[] message);
        void Publish(byte[] message, string routingKey);
        void Publish(byte[] message, string exchange, string routingKey);
        void Publish(byte[] outputData, string exchangeName, string[] routingKeys);
    }
}
