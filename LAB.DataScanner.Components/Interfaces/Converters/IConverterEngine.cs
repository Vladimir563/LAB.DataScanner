using RabbitMQ.Client.Events;

namespace LAB.DataScanner.Components.Interfaces.Converters
{
    //TODO: Why are there methods that are not used?
    public interface IConverterEngine <T,P>
    {
        void Start();
        T Convert(P content);
        void OnReceive(object model, BasicDeliverEventArgs ea);
    }
}
