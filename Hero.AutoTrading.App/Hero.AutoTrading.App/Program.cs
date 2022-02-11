using Hero.AutoTrading.Bitkub;
using Hero.AutoTrading.Bitkub.Services;
using Hero.AutoTrading.Domain.Contracts;
using Hero.AutoTrading.Domain.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Hero.AutoTrading.App
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            ConfigureServices(services, configuration);
            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateAsyncScope();
            var monitor = scope.ServiceProvider.GetRequiredService<ServiceMonitor>();
            await monitor.Invoke();
        }
        private static void ConfigureServices(IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            // Application context
            services.AddHttpClient();
            services.AddSingleton<IConfiguration>(configurationRoot);
            services.AddSingleton<ServiceMonitor>();

            // Bitkub
            services.AddTransient<IBitkubHttpService, BitkubHttpService>();

            // Business domain
            services.AddScoped<IAssetsRebalancing, AssetsRebalancing>();
        }
    }
}
