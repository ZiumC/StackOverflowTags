
using System;
using System.Net;

namespace StackOverflowTags.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _client;
        public HttpService()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All
            };

            _client = new HttpClient();
        }

        public async Task<string> DoGetAsync(string url)
        {
            using HttpResponseMessage response = await _client.GetAsync(url);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
