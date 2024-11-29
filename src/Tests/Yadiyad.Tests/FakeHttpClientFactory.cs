using System.Net.Http;

namespace YadiYad.Tests
{
    public class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;
        public HttpClient CreateClient(string name)
        {
            return _httpClient;
        }

        public FakeHttpClientFactory(HttpClient client)
        {
            _httpClient = client;
        }
    }
}