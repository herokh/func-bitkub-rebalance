using Hero.AutoTrading.Bitkub.DTOs;
using Hero.AutoTrading.Bitkub.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hero.AutoTrading.Bitkub.Services
{
    public class BitkubHttpService : BitkubHttpServiceBase, IBitkubHttpService
    {
        private readonly bool _testMode;
        public BitkubHttpService(IOptions<BitkubConfiguration> bitkubConfiguration,
            IHttpClientFactory httpClientFactory) 
            : base(bitkubConfiguration, httpClientFactory)
        {
            _testMode = Convert.ToBoolean(bitkubConfiguration.Value.TestMode);
        }

        public async Task<string> CreateBuyOrder(string symbol, decimal amount, EnumOrderType orderType)
        {
            var endpoint = _testMode ? "/api/market/place-bid/test" : "/api/market/place-bid";
            return await CreateOrderInternal(endpoint, symbol, amount, orderType);
        }

        public async Task<string> CreateSellOrder(string symbol, decimal amount, EnumOrderType orderType)
        {
            var endpoint = _testMode ? "/api/market/place-ask/test" : "/api/market/place-ask-by-fiat";
            return await CreateOrderInternal(endpoint, symbol, amount, orderType);
        }

        private async Task<string> CreateOrderInternal(string endpoint, string symbol, decimal amount, EnumOrderType orderType)
        {
            var req = new BitkubCreateOrderRequest(symbol, amount, orderType);
            var document = await ExecuteSecureEndpointJson(endpoint, req);
            return document.RootElement.ToString();
        }

        public async Task<IDictionary<string, decimal>> GetAvailableBalancesAsync()
        {
            var endpoint = "/api/market/wallet";
            var req = new BitkubRequest();
            var document = await ExecuteSecureEndpointJson(endpoint, req);
            var balances = new Dictionary<string, decimal>();
            foreach (var item in document.RootElement.GetProperty("result").EnumerateObject())
            {
                var balance = item.Value.GetDecimal();
                if (balance > 0)
                    balances.Add(item.Name, balance);
            }
            return balances;
        }

        public async Task<IDictionary<string, BitkubTickerResponse>> GetMarketTickers(string symbol = null)
        {
            var endpoint = "/api/market/ticker";
            var q = QueryString.Empty;
            if (!string.IsNullOrEmpty(symbol))
            {
                q = q.Add("sym", symbol);
            }
            var document = await ExecutePublicEndpointJson(endpoint + q.ToString());
            var tickers = new Dictionary<string, BitkubTickerResponse>();
            foreach (var item in document.RootElement.EnumerateObject())
            {
                tickers.Add(item.Name, 
                    new BitkubTickerResponse
                {
                    HighestBid = item.Value.GetProperty("highestBid").GetDecimal(),
                    LowestAsk = item.Value.GetProperty("lowestAsk").GetDecimal(),
                });
            }
            return tickers;
        }
    }
}
