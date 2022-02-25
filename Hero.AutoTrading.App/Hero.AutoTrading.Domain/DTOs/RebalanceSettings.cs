namespace Hero.AutoTrading.Domain.DTOs
{
    public class RebalanceSettings
    {
        public string CryptoSymbol { get; set; }
        public string StableSymbol { get; set; }
        public string TickerSymbol { get; set; }
        public decimal MinimumAmountOrder { get; set; }
    }
}
