using Hero.AutoTrading.Domain.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hero.AutoTrading.App
{
    public class ServiceMonitor
    {
        private readonly IAssetsRebalancing _assetsRebalancing;
        private readonly int _executionDelay;

        public ServiceMonitor(IConfiguration configuration,
            IAssetsRebalancing assetsRebalancing)
        {
            _executionDelay = int.Parse(configuration["SystemSettings:ExecutionDelay"]);
            _assetsRebalancing = assetsRebalancing;
        }

        public async Task Invoke(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Start the application...");

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                Console.WriteLine($"Sleep in {_executionDelay / 1000} seconds.");
                await Task.Delay(_executionDelay);

                try
               {
                    Console.WriteLine("Execute started.");

                    var logging = await _assetsRebalancing.Rebalance();

                    Console.WriteLine(logging);

                    Console.WriteLine("Execute completed.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Execute failed.");

                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
