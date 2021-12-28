using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace LAB.DataScanner.Components.Tests.Unit.Services.MessageBroker
{
    public interface IEventingBasicConsumer
    {
        event EventHandler<BasicDeliverEventArgs> Received;
    }
}
