using System;
using JetBrains.Annotations;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.Sdk;
using Lykke.Service.DepositAccumulation.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lykke.Common.Log;
using Lykke.Logs;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Autofac;
using Lykke.Service.DepositAccumulation.Message;

namespace Lykke.Service.DepositAccumulation
{
    [UsedImplicitly]
    public class Startup
    {
        public IContainer ApplicationContainer { get; private set; }

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "DepositAccumulation API";
                options.Logs = logs =>
                {
                    logs.AzureTableName = "DepositAccumulationLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.DepositAccumulationService.Db.LogsConnString;

                    // TODO: You could add extended logging configuration here:
                    /* 
                    logs.Extended = extendedLogs =>
                    {
                        // For example, you could add additional slack channel like this:
                        extendedLogs.AddAdditionalSlackChannel("DepositAccumulation", channelOptions =>
                        {
                            channelOptions.MinLogLevel = LogLevel.Information;
                        });
                    };
                    */
                };

                // TODO: You could add extended Swagger configuration here:
                /*
                options.Swagger = swagger =>
                {
                    swagger.IgnoreObsoleteActions();
                };
                */
            });
        }

        [UsedImplicitly]
        public async void Configure(IApplicationBuilder app, CashTransferMessagesHandler mh, IApplicationLifetime appLifetime)
        {
            app.UseLykkeConfiguration();


            await mh.StartAsync();
        }

        private async Task StartApplication()
        {
            try
            {
                //StartSubscribers();

                await ApplicationContainer.Resolve<IStartupManager>().StartAsync();
                //await Log.WriteMonitorAsync("", Program.EnvInfo, "Started");
            }
            catch (Exception ex)
            {
                //await Log.WriteFatalErrorAsync(nameof(Startup), nameof(StartApplication), "", ex);
                throw;
            }
        }



    }
}
