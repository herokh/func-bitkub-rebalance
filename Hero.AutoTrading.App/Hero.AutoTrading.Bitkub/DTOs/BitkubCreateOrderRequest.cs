using Hero.AutoTrading.Bitkub.Enums;
using Hero.AutoTrading.Bitkub.Utils;
using System.Text.Json.Serialization;

namespace Hero.AutoTrading.Bitkub.DTOs
{
    public class BitkubCreateOrderRequest : BitkubRequest
    {
        public BitkubCreateOrderRequest(string symbol,
            decimal amount,
            EnumOrderType orderType)
        {
            Symbol = symbol;
            Amount = amount;
            Rate = amount;
            OrderType = EnumUtil.GetEnumDescription(orderType);
        }

        [JsonPropertyName("sym")]
        public string Symbol { get; }
        [JsonPropertyName("amt")]
        public decimal Amount { get; }
        [JsonPropertyName("rat")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Rate { get; }
        [JsonPropertyName("typ")]
        public string OrderType { get; }
    }
}
