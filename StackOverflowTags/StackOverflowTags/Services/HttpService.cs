
using Newtonsoft.Json.Linq;
using StackOverflowTags.Models.DatabaseModels;
using System;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace StackOverflowTags.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        public HttpService()
        {
            var httpClientHandler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            _httpClient = new HttpClient(httpClientHandler);

        }

        public async Task<string> DoGetAsync(string url)
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(url);

            string message = "";
            if (response.IsSuccessStatusCode)
            {
                message = await response.Content.ReadAsStringAsync();
            }
            else
            {
                //NEED TO ADD LOGGING LATER!
            }

            return message;
        }
    }
}
