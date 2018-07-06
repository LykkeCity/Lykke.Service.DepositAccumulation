using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public class AccumulatedDepositWaitingForProcessEntity : TableEntity
    {
        public int Generation0ProcessAttempts { get; set; }
    }
}
