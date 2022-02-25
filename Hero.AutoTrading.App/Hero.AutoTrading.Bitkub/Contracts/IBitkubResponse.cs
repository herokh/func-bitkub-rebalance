using System.Text.Json.Serialization;

namespace Hero.AutoTrading.BitkuBb.Contracts
{
    public interface IBitkubResponse
    {
        [JsonPropertyName("error")]
        int? ErrorCode { get; set; }
    }
}
