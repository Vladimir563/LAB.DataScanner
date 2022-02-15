using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace LAB.DataScanner.Components.Interfaces.Downloaders
{
    //TODO: almost the same structure as in the IConverterEngine interface
    public interface IDownloaderEngine
    {
        void StartEngine();

        void OnReceive(object model, BasicDeliverEventArgs ea);
    }
}
