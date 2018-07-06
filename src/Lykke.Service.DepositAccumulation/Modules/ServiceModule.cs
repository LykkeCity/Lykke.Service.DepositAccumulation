using Autofac;
using AzureStorage.Tables;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Sdk;
using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.DepositAccumulation.Core;
using Lykke.Service.DepositAccumulation.Message;
using Lykke.Service.DepositAccumulation.Services;
using Lykke.Service.DepositAccumulation.Settings;
using Lykke.Service.RateCalculator.Client;
using Lykke.SettingsReader;

namespace Lykke.Service.DepositAccumulation.Modules
{    
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var paymentTransactionsConnStr = _appSettings.Nested(x => x.DepositAccumulationService.Db.PaymentTransactionsConnectionString);


            builder.Register(c => AzureTableStorage<PaymentTransactionEntity>.Create(paymentTransactionsConnStr, "PaymentTransactions", c.Resolve<ILogFactory>()));
            builder.RegisterType<PaymentTransactionsRepository>().As<IPaymentTransactionsRepository>();

            builder.Register(c => AzureTableStorage<ProcessedPaymentTransactionEntity>.Create(paymentTransactionsConnStr, "AccumulatedDepositsTransactions", c.Resolve<ILogFactory>()));
            builder.RegisterType<ProcessedPaymentTransactionsRepository>().As<IProcessedPaymentTransactionsRepository>();

            builder.Register(c => AzureTableStorage<AccumulatedDepositEntity>.Create(paymentTransactionsConnStr, "AccumulatedDeposits", c.Resolve<ILogFactory>()));

            builder.Register(c => AzureTableStorage<AccumulatedDepositWaitingForProcessEntity>.Create(paymentTransactionsConnStr, "AccumulatedDepositsWaitingForProcess", c.Resolve<ILogFactory>()));

            builder.Register(c => AzureTableStorage<AccumulatedDepositPeriodEntity>.Create(paymentTransactionsConnStr, "AccumulatedDeposits", c.Resolve<ILogFactory>()));


            builder.RegisterType<AccumulatedDepositRepository>().As<IAccumulatedDepositRepository>();


            builder.RegisterType<DepositAccumulationCalculationService>();
            builder.RegisterType<DepositAccumulationService>();



            builder.RegisterRateCalculatorClient(_appSettings.CurrentValue.RateCalculatorServiceClient.ServiceUrl);



            var rabbitMq = _appSettings.CurrentValue.RabbitMq;
            var connectionString = $"amqp://{rabbitMq.Username}:{rabbitMq.Password}@{rabbitMq.ExternalHost}:{rabbitMq.Port}";
            string exchangeName = _appSettings.CurrentValue.RabbitMq.ExchangeTransfer;
            var subscriberSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = connectionString,
                ExchangeName = exchangeName,
                QueueName = exchangeName + ".DepositAccumulation",
                IsDurable = true,
            };
            builder.RegisterInstance(subscriberSettings).SingleInstance();
            builder.RegisterType<CashTransferMessagesHandler>().SingleInstance();



        }
    }
}
