using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.DepositAccumulation.Core
{
    public class AppSettings
    {
        [Required]
        public DepositAccumulationSettings DepositAccumulationService { get; set; }

        [Required]
        public SlackNotificationsSettings SlackNotifications { get; set; }

        [Required]
        public RabbitMq RabbitMq { get; set; }
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
        [Required(AllowEmptyStrings = false)]
        public string ConnectionString { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string TableName { get; set; }
    }

    public class AzureQueueSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }

    public class RabbitMq
    {
        [Required]
        public string Host { get; set; }

        [Required]
        public string ExternalHost { get; set; }

        [Required]
        public string Port { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ExchangeTransfer { get; set; }


    }

}
