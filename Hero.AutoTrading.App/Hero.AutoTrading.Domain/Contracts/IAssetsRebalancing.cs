using System.Threading.Tasks;

namespace Hero.AutoTrading.Domain.Contracts
{
    public interface IAssetsRebalancing
    {
        Task<string> Rebalance();
    }
}
