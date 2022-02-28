using System.Text.Json.Serialization;

namespace Hero.AutoTrading.Notification.DTOs
{
    public class LineMessage
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
