using System;
using System.Text.Json.Serialization;

namespace Hero.AutoTrading.Bitkub.DTOs
{
    public class BitkubRequest : IBitkubRequest
    {
        [JsonPropertyName("ts")]
        public int TimeStamp { get; private set; }
        [JsonPropertyName("sig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Signature { get; private set; }

        public void SetTimeStampToCurrentTime()
        {
            TimeStamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public void UpdateSignature(string signature)
        {
            Signature = signature;
        }
    }
}
