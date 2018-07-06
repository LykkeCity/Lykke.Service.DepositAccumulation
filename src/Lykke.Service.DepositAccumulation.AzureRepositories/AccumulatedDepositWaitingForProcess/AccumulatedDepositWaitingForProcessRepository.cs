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
            INoSQLTableStorage<AccumulatedDepositWaitingForProcessEntity> tableStorage
            )
        {
            _tableStorage = tableStorage;
        }

        public async Task<IEnumerable<AccumulatedDepositWaitingForProcessEntity>> GetWaitingForProcessAsync(int generation)
        {
            return await _tableStorage.GetDataAsync(GeneratePartition(generation));
        }

        public async Task SaveWaitingForProcessAsync(string clientId, string transactionId, int generation)
        {
            await _tableStorage.CreateIfNotExistsAsync(
                new AccumulatedDepositWaitingForProcessEntity
                {
                    PartitionKey = GeneratePartition(generation),
                    RowKey = $"{clientId}|{transactionId}"
                });
        }

        public async Task DeleteWaitingForProcessAsync(string clientId, string transactionId)
        {
            await _tableStorage.DeleteIfExistAsync(clientId, transactionId);
        }

        private string GeneratePartition(int generation)
        {
            return $"Generation{generation}";
        }

        public async Task IncrementProcessingAttempt(AccumulatedDepositWaitingForProcessEntity entity)
        {
            entity.Generation0ProcessAttempts++;
            await _tableStorage.ReplaceAsync(entity);
        }

        public async Task MoveToNextGeneration(AccumulatedDepositWaitingForProcessEntity entity)
        {
            string partitionKey = entity.PartitionKey;
            string rowKey = entity.RowKey;

            entity.PartitionKey = GeneratePartition(1);
            await _tableStorage.InsertOrReplaceAsync(entity);

            await _tableStorage.DeleteIfExistAsync(partitionKey, rowKey);
        }


    }
}
