using Hero.AutoTrading.Bitkub;
using Hero.AutoTrading.Bitkub.DTOs;
using Hero.AutoTrading.Bitkub.Services;
using Hero.AutoTrading.Domain.Contracts;
using Hero.AutoTrading.Domain.DTOs;
using Hero.AutoTrading.Domain.Implementations;
using Hero.AutoTrading.Notification.Contracts;
using Hero.AutoTrading.Notification.DTOs;
using Hero.AutoTrading.Notification.Implementations;
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

            builder.Services.AddOptions<LineMessagingConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("LineMessagingConfiguration").Bind(settings);
                });

            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IBitkubHttpService, BitkubHttpService>();
            builder.Services.AddScoped<INotificationService, LineNotificationService>();
            builder.Services.AddScoped<IAssetsRebalancing, AssetsRebalancing>();
        }
    }
}
