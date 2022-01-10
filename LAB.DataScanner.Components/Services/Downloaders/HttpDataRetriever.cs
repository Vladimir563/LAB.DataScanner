using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LAB.DataScanner.Components.Services.Downloaders
{
    public class HttpDataRetriever : IDataRetriever, IDisposable
    {
        private readonly HttpClient _httpClient;

        public HttpDataRetriever(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public async Task<byte[]> RetrieveBytesAsync(string uri)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(uri);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsByteArrayAsync();

                return responseBody;
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

        public async Task<string> RetrieveStringAsync(string uri)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(uri);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }
    }
}