using Hero.AutoTrading.Bitkub;
using Hero.AutoTrading.Bitkub.DTOs;
using Hero.AutoTrading.Bitkub.Services;
using Hero.AutoTrading.Domain.Contracts;
using Hero.AutoTrading.Domain.DTOs;
using Hero.AutoTrading.Domain.Implementations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Hero.AutoTrading.ScheduleFunction.Startup))]
namespace Hero.AutoTrading.ScheduleFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<BitkubConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("BitkubConfiguration").Bind(settings);
                });

            builder.Services.AddOptions<RebalanceSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("RebalanceSettings").Bind(settings);
                });

            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IBitkubHttpService, BitkubHttpService>();
            builder.Services.AddScoped<IAssetsRebalancing, AssetsRebalancing>();
        }
    }
}
