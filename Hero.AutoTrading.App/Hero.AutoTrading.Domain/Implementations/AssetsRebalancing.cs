using Hero.AutoTrading.Bitkub;
using Hero.AutoTrading.Bitkub.Enums;
using Hero.AutoTrading.Domain.Contracts;
using Hero.AutoTrading.Domain.DTOs;
using Hero.AutoTrading.Notification.Contracts;
using Hero.AutoTrading.Notification.DTOs;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hero.AutoTrading.Domain.Implementations
{
    public class AssetsRebalancing : IAssetsRebalancing
    {
        private readonly IBitkubHttpService _bitkubHttpService;
        private readonly INotificationService _notificationService;
        private readonly RebalanceSettings _rebalanceSettings;
        private readonly LineMessagingConfiguration _lineMessagingConfiguration;

        private readonly string _cryptoSymbol;
        private readonly string _stableSymbol;
        private readonly string _tickerSymbol;
        private readonly decimal _minimumAmountOrder;
        private readonly StringBuilder _loggingBuilder;

        public AssetsRebalancing(IBitkubHttpService bitkubHttpService,
             INotificationService notificationService,
             IOptions<RebalanceSettings> rebalanceSettings,
             IOptions<LineMessagingConfiguration> lineMessagingConfiguration)
        {
            _bitkubHttpService = bitkubHttpService;
            _notificationService = notificationService;
            _rebalanceSettings = rebalanceSettings.Value;
            _lineMessagingConfiguration = lineMessagingConfiguration.Value;
            _cryptoSymbol = _rebalanceSettings.CryptoSymbol ?? string.Empty;
            _stableSymbol = _rebalanceSettings.StableSymbol ?? string.Empty;
            _minimumAmountOrder = Convert.ToDecimal(_rebalanceSettings.MinimumAmountOrder);
            _tickerSymbol = _rebalanceSettings.TickerSymbol ?? string.Empty;
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
                .FirstOrDefault(x => x.Key.Equals(_cryptoSymbol))
                .Value;
            var stableCoinBalance = availableBalances
                .FirstOrDefault(x => x.Key.Equals(_stableSymbol))
                .Value;

            var tickers = await _bitkubHttpService.GetMarketTickers(_tickerSymbol);
            var tickerInfo = tickers.First().Value;
            var averagePrice = (tickerInfo.HighestBid + tickerInfo.LowestAsk) / 2;

            _loggingBuilder.AppendLine($"Ticker {_tickerSymbol}");
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

                if (diffSell > _minimumAmountOrder)
                {
                    var sellResult = await _bitkubHttpService.CreateSellOrder(_tickerSymbol, diffSell, EnumOrderType.Market);

                    _loggingBuilder.AppendLine($"Sell result {sellResult}");
                }
                else
                {
                    _loggingBuilder.AppendLine($"Skip to create sell order. The amount is lower than {_minimumAmountOrder}");
                }
            }
            else if (cryptoValue < (rebalanceMark - rebalanceRate))
            {
                _loggingBuilder.AppendLine("Starting to create buy order");

                var diffBuy = rebalanceMark - cryptoValue;

                _loggingBuilder.AppendLine($"Buy price {diffBuy.ToString("C")}");

                if (diffBuy > _minimumAmountOrder)
                {
                    var buyResult = await _bitkubHttpService.CreateBuyOrder(_tickerSymbol, diffBuy, EnumOrderType.Market);

                    _loggingBuilder.AppendLine($"Buy result {buyResult}");
                }
                else
                {
                    _loggingBuilder.AppendLine($"Skip to create buy order. The amount is lower than {_minimumAmountOrder}");
                }
            }
            else
            {
                _loggingBuilder.AppendLine("Skip for trading");

                // do nothing
            }

            var log = _loggingBuilder.ToString();
            var notificationMessage = CreateNotificationMessage(log);
            await _notificationService.PushMessages(notificationMessage);

            return log;
        }

        private NotificationMessage CreateNotificationMessage(string text)
        {
            var to = new string[] { _lineMessagingConfiguration.OwnerUserId };
            var messages = new List<LineMessage>
                    {
                        new LineMessage
                        {
                            Type = "text",
                            Text = text
                        }
                    }.ToArray();


            var notificationMessage = new NotificationMessage
            {
                To = to,
                Messages = messages
            };

            return notificationMessage;
        }
    }
}
