using System.Text.Json.Serialization;

namespace Hero.AutoTrading.Notification.DTOs
{
    public class NotificationMessage : INotificationRequest
    {
        [JsonPropertyName("to")]
        public string[] To { get; set; }

        [JsonPropertyName("messages")]
        public LineMessage[] Messages { get; set; }
    }
}
