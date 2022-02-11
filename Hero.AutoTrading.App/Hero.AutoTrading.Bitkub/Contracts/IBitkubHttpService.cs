using Hero.AutoTrading.Bitkub.DTOs;
using Hero.AutoTrading.Bitkub.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hero.AutoTrading.Bitkub
{
    public interface IBitkubHttpService
    {
        Task<IDictionary<string, decimal>> GetAvailableBalancesAsync();

        Task<IDictionary<string, BitkubTickerResponse>> GetMarketTickers(string symbol = null);

        Task<string> CreateBuyOrder(string symbol, decimal amount, EnumOrderType orderType);

        Task<string> CreateSellOrder(string symbol, decimal amount, EnumOrderType orderType);
    }
}
