using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.Service.DepositAccumulation.Core;
using Lykke.Service.DepositAccumulation.Modules;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Newtonsoft.Json;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker;
using Lykke.Domain.Prices.Model;
using Common;
using Lykke.Service.DepositAccumulation.Message;
using Lykke.Service.DepositAccumulation.AzureRepositories;
using AzureStorage;

namespace Lykke.Service.DepositAccumulation
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; set; }
        public IConfigurationRoot Configuration { get; }

        private IStopable _cashTransfersSubscriber;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

            services.AddSwaggerGen(options =>
            {
                options.DefaultLykkeConfiguration("v1", "DepositAccumulation API");
            });

            string settingsUrl = Configuration.GetValue<string>("SettingsUrl");
            var appSettings = HttpSettingsLoader.Load<AppSettings>(settingsUrl);

            var builder = new ContainerBuilder();
            ILog log = CreateLogWithSlack(services, appSettings);
            builder.RegisterModule(new ServiceModule(log));

            builder.RegisterInstance<AppSettings>(appSettings).SingleInstance();

            ConfigureComponents(builder, appSettings, log);

            builder.Populate(services);

            AssemblyLoadContext.Default.Unloading += ctx =>
            {
                ShutDownHandler(log);
            };

            builder.RegisterInstance<INoSQLTableStorage<PaymentTransactionEntity>>(new AzureTableStorage<PaymentTransactionEntity>(appSettings.DepositAccumulationService.Db.PaymentTransactionsConnectionString, "PaymentTransactions", log));
            builder.RegisterType<PaymentTransactionsRepository>().As<IPaymentTransactionsRepository>();

            builder.RegisterInstance<INoSQLTableStorage<AccumulatedDepositEntity>>(new AzureTableStorage<AccumulatedDepositEntity>(appSettings.DepositAccumulationService.Db.PaymentTransactionsConnectionString, "AccumulatedDeposits", log));
            builder.RegisterType<AccumulatedDepositRepository>().As<IAccumulatedDepositRepository>();


            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseLykkeMiddleware("DepositAccumulation", ex => new {Message = "Technical problem"});

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUi();

            appLifetime.ApplicationStopped.Register(() =>
            {
                ApplicationContainer.Dispose();
            });
        }

        private static ILog CreateLogWithSlack(IServiceCollection services, AppSettings settings)
        {
            LykkeLogToAzureStorage logToAzureStorage = null;

            var logToConsole = new LogToConsole();
            var logAggregate = new LogAggregate();

            logAggregate.AddLogger(logToConsole);

            var dbLogConnectionString = settings.DepositAccumulationService.Db.LogsConnString;

            // Creating azure storage logger, which logs own messages to concole log
            if (!string.IsNullOrEmpty(dbLogConnectionString) && !(dbLogConnectionString.StartsWith("${") && dbLogConnectionString.EndsWith("}")))
            {
                logToAzureStorage = new LykkeLogToAzureStorage("Lykke.Service.DepositAccumulation", new AzureTableStorage<LogEntity>(
                    dbLogConnectionString, "DepositAccumulationLog", logToConsole));

                logAggregate.AddLogger(logToAzureStorage);
            }

            // Creating aggregate log, which logs to console and to azure storage, if last one specified
            var log = logAggregate.CreateLogger();

            // Creating slack notification service, which logs own azure queue processing messages to aggregate log
            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(new AzureQueueIntegration.AzureQueueSettings
            {
                ConnectionString = settings.SlackNotifications.AzureQueue.ConnectionString,
                QueueName = settings.SlackNotifications.AzureQueue.QueueName
            }, log);

            // Finally, setting slack notification for azure storage log, which will forward necessary message to slack service
            logToAzureStorage?.SetSlackNotification(slackService);

            return log;
        }


        private void ConfigureComponents(
            ContainerBuilder builder,
            AppSettings appSettings,
            ILog log)
        {
            var rabbitMq = appSettings.RabbitMq;

            string exchangeName = appSettings.RabbitMq.ExchangeTransfer;
            var subscriberSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = $"amqp://{rabbitMq.Username}:{rabbitMq.Password}@{rabbitMq.ExternalHost}:{rabbitMq.Port}",
                ExchangeName = exchangeName,
                QueueName = exchangeName + ".DepositAccumulation",
                IsDurable = true,
            };
            var errorStrategy = new DefaultErrorHandlingStrategy(log, subscriberSettings);
            var cashTransfersSubscriber = new RabbitMqSubscriber<CashTransferOperation>(subscriberSettings, errorStrategy, 1000);
            _cashTransfersSubscriber = cashTransfersSubscriber;
            builder
                .RegisterInstance(_cashTransfersSubscriber)
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();
            var paymentTransactionsRepository = new PaymentTransactionsRepository(appSettings, log);
            var accumulatedDepositRepository = new AccumulatedDepositRepository(appSettings, log);
            var cashTransferMesageHandler = new SeparatingMessagesHandler(
                cashTransfersSubscriber,
                paymentTransactionsRepository,
                accumulatedDepositRepository,
                log);
            builder.RegisterInstance(cashTransferMesageHandler).SingleInstance();


        }

        private void ShutDownHandler(ILog log)
        {
            try
            {
                var tasks = new Task[]
                {
                    Task.Run(() =>
                    {
                        _cashTransfersSubscriber.Stop();
                        //_cashTransfersCollector.SaveAsync().Wait();
                    })
                };
                Task.WaitAll(tasks);
            }
            catch (AggregateException exc)
            {
                log.WriteErrorAsync(
                    nameof(DepositAccumulation),
                    nameof(Startup),
                    nameof(ShutDownHandler),
                    exc.Flatten(),
                    DateTime.UtcNow)
                .Wait();
            }
        }


    }
}
