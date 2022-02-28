using Hero.AutoTrading.Notification.DTOs;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Hero.AutoTrading.Notification.Implementations
{
    public abstract class LineNotificationServiceBase
    {
        private readonly LineMessagingConfiguration _lineMessagingConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;

        protected LineNotificationServiceBase(IOptions<LineMessagingConfiguration> options,
            IHttpClientFactory httpClientFactory)
        {
            _lineMessagingConfiguration = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        protected virtual async Task PostAsync<T>(string url, T requestBody) where T : INotificationRequest
        {
            HttpClient httpClient = CreateHttpClient();
            string serializedBody = JsonSerializer.Serialize<T>(requestBody);
            var content = new StringContent(serializedBody, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        protected virtual HttpClient CreateHttpClient()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();

            httpClient.BaseAddress = new Uri(_lineMessagingConfiguration.BaseUrl);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_lineMessagingConfiguration.AccessToken}");

            return httpClient;
        }
    }
}
