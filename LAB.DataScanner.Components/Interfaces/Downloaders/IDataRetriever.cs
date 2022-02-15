using System.Threading.Tasks;

namespace LAB.DataScanner.Components.Services.Downloaders
{
    //TODO: Why are there methods that are not used?
    public interface IDataRetriever
    {
        Task<byte[]> RetrieveBytesAsync(string uri);
        Task<string> RetrieveStringAsync(string uri);
    }
}
