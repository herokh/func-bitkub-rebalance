using Hero.AutoTrading.Bitkub.DTOs;
using Hero.AutoTrading.Bitkub.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hero.AutoTrading.Bitkub.Services
{
    public abstract class BitkubHttpServiceBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public BitkubHttpServiceBase(IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public virtual async Task<JsonDocument> ExecuteSecureEndpointJson<T>(string endpoint, T request = default) where T : IBitkubRequest
        {
            var res = await ExecuteSecureEndpoint(endpoint, request);
            var resString = JsonSerializer.Serialize(res);
            var document = JsonDocument.Parse(resString);
            return document;
        }

        public virtual async Task<JsonDocument> ExecutePublicEndpointJson(string endpoint, IBitkubRequest request = default)
        {
            var res = await ExecutePublicEndpoint(endpoint, request);
            var resString = JsonSerializer.Serialize(res);
            var document = JsonDocument.Parse(resString);
            return document;
        }

        public virtual async Task<object> ExecuteSecureEndpoint<T>(string endpoint, T request = default) where T : IBitkubRequest
        {
            var baseUrl = _configuration["Bitkub:BaseUrl"];
            var apiKey = _configuration["Bitkub:ApiKey"];
            var secretKey = _configuration["Bitkub:ApiSecret"];

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            //client.DefaultRequestHeaders.Add("Content-type", "application/json");
            client.DefaultRequestHeaders.Add("X-BTK-APIKEY", apiKey);

            request.SetTimeStampToCurrentTime();
            var reqSerialized = JsonSerializer.Serialize(request);
            var signature = CryptographyUtil.HashHMACSHA256(secretKey, reqSerialized);
            request.UpdateSignature(signature);
            var content = JsonSerializer.Serialize(request);

            var payload = new StringContent(content, Encoding.UTF8, "application/json");

            var res = await client.PostAsync(endpoint, payload);

            using var resStream = await res.Content.ReadAsStreamAsync();
            var message = await JsonSerializer.DeserializeAsync<object>(resStream);

            if (res.IsSuccessStatusCode)
            {
                return message;
            }
            else
            {
                throw new InvalidOperationException(JsonSerializer.Serialize(message));
            }
        }

        public virtual async Task<object> ExecutePublicEndpoint(string endpoint, IBitkubRequest request = default)
        {
            var baseUrl = _configuration["Bitkub:BaseUrl"];

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            //client.DefaultRequestHeaders.Add("Content-type", "application/json");

            var res = await client.GetAsync(endpoint);

            using var resStream = await res.Content.ReadAsStreamAsync();
            var message = await JsonSerializer.DeserializeAsync<object>(resStream);

            if (res.IsSuccessStatusCode)
            {
                return message;
            }
            else
            {
                throw new InvalidOperationException(JsonSerializer.Serialize(message));
            }
        }
    }
}
