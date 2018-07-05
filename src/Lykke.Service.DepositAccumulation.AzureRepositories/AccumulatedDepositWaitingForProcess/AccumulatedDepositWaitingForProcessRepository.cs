using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.DepositAccumulation.Core;
using Lykke.Service.DepositAccumulation.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public class AccumulatedDepositWaitingForProcessRepository : IAccumulatedDepositWaitingForProcessRepository
    {
        //private readonly string _totalRowKey = "Total";

        private readonly INoSQLTableStorage<AccumulatedDepositWaitingForProcessEntity> _tableStorage;

        public AccumulatedDepositWaitingForProcessRepository(
            INoSQLTableStorage<AccumulatedDepositWaitingForProcessEntity> tableStorage,
            ILog log
            )
        {
            _tableStorage = tableStorage;
        }

        public async Task<IEnumerable<AccumulatedDepositWaitingForProcessEntity>> GetWaitingForProcessAsync(string clientId)
        {
            return await _tableStorage.GetDataAsync(clientId);
        }

        public async Task SaveWaitingForProcessAsync(string clientId, string transactionId)
        {
            await _tableStorage.CreateIfNotExistsAsync(
                new AccumulatedDepositWaitingForProcessEntity
                {
                    PartitionKey = clientId,
                    RowKey = transactionId
                });
        }
        public async Task DeleteWaitingForProcessAsync(string clientId, string transactionId)
        {
            await _tableStorage.DeleteIfExistAsync(clientId, transactionId);
        }


    }
}
