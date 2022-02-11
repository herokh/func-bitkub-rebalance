using System.ComponentModel;

namespace Hero.AutoTrading.Bitkub.Enums
{
    public enum EnumOrderType
    {
        [Description("")]
        Unknown = 0,
        [Description("limit")]
        Limit = 1,
        [Description("market")]
        Market = 2
    }
}
