using Lykke.Service.DepositAccumulation.Core.Domain;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public class AccumulatedDepositPeriodEntity : TableEntity, IAccumulatedDepositPeriod
    {
        public DateTime StartDateTime { get; set; }
        
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public double Amount { get; set; }
        public double AmountInUsd { get; set; }

    }
}
