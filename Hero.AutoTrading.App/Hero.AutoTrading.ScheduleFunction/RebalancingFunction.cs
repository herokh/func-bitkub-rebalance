using Hero.AutoTrading.Domain.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Hero.AutoTrading.ScheduleFunction
{
    public class RebalancingFunction
    {
        private readonly IAssetsRebalancing _assetsRebalancing;

        public RebalancingFunction(IAssetsRebalancing assetsRebalancing)
        {
            _assetsRebalancing = assetsRebalancing;
        }

        [FunctionName("RebalancingFunction")]
        public async Task RunAsync([TimerTrigger("*/5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

                var result = await _assetsRebalancing.Rebalance();
                log.LogInformation(result);
            }
            catch (System.Exception e)
            {
                log.LogError(e.Message);
                throw;
            }
        }
    }
}
