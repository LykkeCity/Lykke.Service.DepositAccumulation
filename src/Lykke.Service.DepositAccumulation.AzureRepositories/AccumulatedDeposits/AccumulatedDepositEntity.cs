using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public class AccumulatedDepositEntity : TableEntity
    {
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public double Amount { get; set; }

    }
}
