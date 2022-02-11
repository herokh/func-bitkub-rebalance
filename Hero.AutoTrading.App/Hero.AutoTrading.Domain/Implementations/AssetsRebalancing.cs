using Hero.AutoTrading.Bitkub;
using Hero.AutoTrading.Bitkub.Enums;
using Hero.AutoTrading.Domain.Contracts;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hero.AutoTrading.Domain.Implementations
{
    public class AssetsRebalancing : IAssetsRebalancing
    {
        private readonly IConfiguration _configuration;
        private readonly IBitkubHttpService _bitkubHttpService;

        private readonly string _rebalanceCrypto;
        private readonly string _rebalanceStableCoin;
        private readonly string _rebalanceTickerSymbol;
        private readonly StringBuilder _loggingBuilder;

        public AssetsRebalancing(IBitkubHttpService bitkubHttpService, 
            IConfiguration configuration)
        {
            _bitkubHttpService = bitkubHttpService;
            _configuration = configuration;
            _rebalanceCrypto = _configuration["RebalanceSettings:crypto"] ?? string.Empty;
            _rebalanceStableCoin = _configuration["RebalanceSettings:stableCoin"] ?? string.Empty;
            _rebalanceTickerSymbol = _configuration["RebalanceSettings:TickerSymbol"] ?? string.Empty;
            _loggingBuilder = new StringBuilder();
        }

        public async Task<string> Rebalance()
        {
            var availableBalances = await _bitkubHttpService.GetAvailableBalancesAsync();

            _loggingBuilder.AppendLine($"Available balances");
            foreach (var item in availableBalances)
            {
                _loggingBuilder.AppendLine($"Currency {item.Key} Balance {item.Value.ToString("C")}");
            }

            var cryptoBalance = availableBalances
                .FirstOrDefault(x => x.Key.Equals(_rebalanceCrypto))
                .Value;
            var stableCoinBalance = availableBalances
                .FirstOrDefault(x => x.Key.Equals(_rebalanceStableCoin))
                .Value;

            var tickers = await _bitkubHttpService.GetMarketTickers(_rebalanceTickerSymbol);
            var tickerInfo = tickers.First().Value;
            var averagePrice = (tickerInfo.HighestBid + tickerInfo.LowestAsk) / 2;

            _loggingBuilder.AppendLine($"Ticker {_rebalanceTickerSymbol}");
            _loggingBuilder.AppendLine($"Highest Bid {tickerInfo.HighestBid.ToString("C")} Lowest Ask {tickerInfo.LowestAsk.ToString("C")}");

            var cryptoValue = cryptoBalance * averagePrice;
            var stableCoinValue = stableCoinBalance * 1;

            _loggingBuilder.AppendLine($"Assets Value");
            _loggingBuilder.AppendLine($"Crypto Value {cryptoValue.ToString("C")}");
            _loggingBuilder.AppendLine($"StableCoin Value {stableCoinValue.ToString("C")}");

            var rebalanceMark = (cryptoValue + stableCoinValue) / 2;
            var rebalancePercent = 1;
            var rebalanceRate = (rebalanceMark * rebalancePercent) / 100;

            _loggingBuilder.AppendLine($"Rebalance Calculation Result");
            _loggingBuilder.AppendLine($"Average {rebalanceMark.ToString("C")}");
            _loggingBuilder.AppendLine($"Rate {rebalanceRate.ToString("C")}");

            _loggingBuilder.AppendLine("Making decision...");

            if (cryptoValue > (rebalanceMark + rebalanceRate))
            {
                _loggingBuilder.AppendLine("Starting to create sell order");

                var diffSell = cryptoValue - rebalanceMark;

                _loggingBuilder.AppendLine($"Sell price {diffSell.ToString("C")}");

                var sellResult = await _bitkubHttpService.CreateSellOrder(_rebalanceTickerSymbol, diffSell, EnumOrderType.Market);

                _loggingBuilder.AppendLine($"Sell result {sellResult}");
            }
            else if (cryptoValue < (rebalanceMark - rebalanceRate))
            {
                _loggingBuilder.AppendLine("Starting to create buy order");

                var diffBuy = rebalanceMark - cryptoValue;

                _loggingBuilder.AppendLine($"Buy price {diffBuy.ToString("C")}");

                var buyResult = await _bitkubHttpService.CreateBuyOrder(_rebalanceTickerSymbol, diffBuy, EnumOrderType.Market);

                _loggingBuilder.AppendLine($"Buy result {buyResult}");
            }
            else
            {
                _loggingBuilder.AppendLine("Skip for trading");
                // do nothing
            }

            return _loggingBuilder.ToString();
        }
    }
}
