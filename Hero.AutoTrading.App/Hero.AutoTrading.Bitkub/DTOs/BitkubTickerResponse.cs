using System.Text.Json.Serialization;

namespace Hero.AutoTrading.Bitkub.DTOs
{
    public class BitkubTickerResponse
    {
        [JsonPropertyName("lowestAsk")]
        public decimal LowestAsk { get; set; }
        [JsonPropertyName("highestBid")]
        public decimal HighestBid { get; set; }
    }
}
