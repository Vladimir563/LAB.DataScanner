using System;
using System.Collections.Generic;
using System.Text;

namespace LAB.DataScanner.ConfigDatabaseApi.Contracts.MessageBroker
{
    public interface IRmqPublisher : IDisposable
    {
        void Publish(byte[] message);
        void Publish(byte[] message, string routingKey);
        void Publish(byte[] message, string exchange, string routingKey);
        void Publish(byte[] outputData, string exchangeName, string[] routingKeys);
    }
}
