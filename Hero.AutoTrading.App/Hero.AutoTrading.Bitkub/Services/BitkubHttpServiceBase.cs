using Hero.AutoTrading.Bitkub.DTOs;
using Hero.AutoTrading.Bitkub.Utils;
using Hero.AutoTrading.BitkuBb.Contracts;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hero.AutoTrading.Bitkub.Services
{
    public abstract class BitkubHttpServiceBase
    {
        private readonly BitkubConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public BitkubHttpServiceBase(IOptions<BitkubConfiguration> bitkubConfiguration,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = bitkubConfiguration.Value;
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
            var baseUrl = _configuration.BaseUrl;
            var apiKey = _configuration.ApiKey;
            var secretKey = _configuration.ApiSecret;

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

            var resString = await res.Content.ReadAsStringAsync();
            var message = JsonSerializer.Deserialize<object>(resString);
            var doc = JsonDocument.Parse(resString);
            var errorCode = doc.RootElement.GetProperty("error").GetInt32();

            if (res.IsSuccessStatusCode && errorCode == 0)
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
            var baseUrl = _configuration.BaseUrl;

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
