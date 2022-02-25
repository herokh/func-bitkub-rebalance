using Hero.AutoTrading.BitkuBb.Contracts;

namespace Hero.AutoTrading.Bitkub.DTOs
{
    public class BitkubResponseBase : IBitkubResponse
    {
        public int? ErrorCode { get; set; }
    }
}
