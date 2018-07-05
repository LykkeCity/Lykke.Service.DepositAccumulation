using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public class PaymentTransactionEntity : TableEntity
    {
        public string ClientId { get; set; }
        public string TransactionId { get; set; }
        public DateTime Created { get; set; }
        public string AssetId { get; set; }
        public double Amount { get; set; }
        public double AmountInUsd { get; set; }
        public double ExchangeRate { get; set; }
        public string Status { get; set; }
        public string PaymentSystem { get; set; }
    }
}
