using RabbitMQ.Client.Events;

namespace LAB.DataScanner.Components.Interfaces.Converters
{
    public interface IConverterEngine <T,P>
    {
        void Start();
        T Convert(P content);
        void OnReceive(object model, BasicDeliverEventArgs ea);
    }
}
