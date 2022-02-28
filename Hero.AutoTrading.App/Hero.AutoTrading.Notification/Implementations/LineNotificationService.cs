using Hero.AutoTrading.Notification.Contracts;
using Hero.AutoTrading.Notification.DTOs;
using Microsoft.Extensions.Options;

namespace Hero.AutoTrading.Notification.Implementations
{
    public class LineNotificationService : LineNotificationServiceBase, INotificationService
    {
        public LineNotificationService(IOptions<LineMessagingConfiguration> options,
            IHttpClientFactory httpClientFactory) :
            base(options, httpClientFactory)
        {
        }

        public async Task PushMessages(NotificationMessage notificationMessage)
        {
            string url = "/v2/bot/message/multicast";
            await PostAsync(url, notificationMessage);
        }
    }
}
