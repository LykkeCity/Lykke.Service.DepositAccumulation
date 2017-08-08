using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public class PaymentTransactionEntity : TableEntity
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string ClientId { get; set; }
        public DateTime Created { get; set; }
        public string Status { get; set; }
        public string PaymentSystem { get; set; }
        public string Info { get; set; }
        public double? Rate { get; set; }
        public string AggregatorTransactionId { get; set; }
        public double Amount { get; set; }
        public string AssetId { get; set; }
        public double? DepositedAmount { get; set; }
        public string DepositedAssetId { get; set; }

    }
}
