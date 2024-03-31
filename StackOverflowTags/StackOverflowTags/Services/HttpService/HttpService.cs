
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

namespace StackOverflowTags.Services.HttpService
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        public HttpService(ILogger<HttpService> logger)
        {
            var httpClientHandler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            _httpClient = new HttpClient(httpClientHandler);
            _logger = logger;
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
                _logger.LogError($"Request status code: {response.StatusCode}, message: {await response.Content.ReadAsStringAsync()}", url);
                throw new Exception("Response status code is wrong");
            }

            return message;
        }
    }
}
