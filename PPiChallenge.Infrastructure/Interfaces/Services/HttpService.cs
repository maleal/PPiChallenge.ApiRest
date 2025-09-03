using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PPiChallenge.Core.Interfaces.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PPiChallenge.Infrastructure.Interfaces.Services
{
    public class HttpService : IHttpService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpService> _logger;

        public HttpService(IHttpClientFactory httpClientFactory, ILogger<HttpService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("GET failed. Status: {StatusCode}. Content: {Content}", response.StatusCode, json);
                    throw new HttpRequestException($"HTTP {response.StatusCode}: {json}");
                }

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during GET request.");
                throw;
            }
        }

        public async Task<T> PostAsync<T>(string url, object data)
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error {StatusCode} - {Content}", response.StatusCode, error);
                    throw new HttpRequestException($"HTTP Status{response.StatusCode} : {error}");
                }

                var raw = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content-Type: {response.Content.Headers.ContentType}");
                Console.WriteLine("Raw response:");
                Console.WriteLine(raw);

                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<T>(raw);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on POST request to {Url}", url);
                throw;
            }
        }

        /*
         Si necesitamos hacer un POST a una URL que tiene los parámetros en el query string, 
         no en el cuerpo, esto es, no hay un body real que enviar, usamos lo siguiente basodo en 
         SendAsync()
         */
        public async Task<T> PostAsync<T>(string url, string queryString = null)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var fullUrl = string.IsNullOrWhiteSpace(queryString) ? url : $"{url}?{queryString}";

                var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
                var response = await client.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("POST failed. Status: {StatusCode}. Content: {Content}", response.StatusCode, json);
                    throw new HttpRequestException($"HTTP {response.StatusCode}: {json}");
                }

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during POST request.");
                throw;
            }

        }
    }
}
