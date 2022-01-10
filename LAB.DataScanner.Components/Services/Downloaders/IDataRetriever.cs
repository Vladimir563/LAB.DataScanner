using System.Threading.Tasks;

namespace LAB.DataScanner.Components.Services.Downloaders
{
    public interface IDataRetriever
    {
        Task<byte[]> RetrieveBytesAsync(string uri);
        Task<string> RetrieveStringAsync(string uri);
    }
}
