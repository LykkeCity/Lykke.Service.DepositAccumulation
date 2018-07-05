using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.DepositAccumulation.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public DepositAccumulationSettings DepositAccumulationService { get; set; }

        public RabbitMq RabbitMq { get; set; }

        public SagasRabbitMq SagasRabbitMq { get; set; }

        public RateCalculatorSettings RateCalculatorServiceClient { get; set; }

    }

    public class RateCalculatorSettings
    {
        public string ServiceUrl { get; set; }
    }

    public class DepositAccumulationSettings
    {
        public DbSettings Db { get; set; }

    }

    public class DbSettings
    {
        public string LogsConnString { get; set; }
        public string PaymentTransactionsConnectionString { get; set; }
    }

    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }

        public int ThrottlingLimitSeconds { get; set; }
    }

    public class AzureTableSettings
    {
        public string ConnectionString { get; set; }

        public string TableName { get; set; }
    }

    public class AzureQueueSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }

    public class RabbitMq
    {
        public string Host { get; set; }

        public string ExternalHost { get; set; }

        public string Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ExchangeTransfer { get; set; }


    }

    public class SagasRabbitMq
    {
        public string RabbitConnectionString { get; set; }

        public string RetryDelay { get; set; }
    }

}





