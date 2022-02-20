using LAB.DataScanner.Components.Interfaces.Engines;

namespace LAB.DataScanner.Components.Interfaces.Converters
{
    public interface IConverterEngine <T,P> : IEngine
    {
        T Convert(P content);
    }
}
