using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.DepositAccumulation.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public class AccumulatedDepositRepository : IAccumulatedDepositRepository
    {
        private readonly INoSQLTableStorage<AccumulatedDepositEntity> _tableStorage;

        public AccumulatedDepositRepository(AppSettings appSettings, ILog log)
        {
            _tableStorage = new AzureTableStorage<AccumulatedDepositEntity>(appSettings.DepositAccumulationService.Db.PaymentTransactionsConnectionString, "AccumulatedDeposits", log);
        }

        public async Task<AccumulatedDepositEntity> GetAsync(string clientId, string assetId)
        {
             return await _tableStorage.GetDataAsync(GeneratePartition(clientId), GenerateRowKey(assetId));
        }

        public async Task SaveAsync(string clientId, string assetId, double amount)
        {
            AccumulatedDepositEntity e = new AccumulatedDepositEntity();
            e.Amount = amount;
            e.AssetId = assetId;
            e.ClientId = clientId;
            e.PartitionKey = GeneratePartition(clientId);
            e.RowKey = GenerateRowKey(assetId);

            await _tableStorage.InsertOrReplaceAsync(e);
        }

        public string GeneratePartition(string clientId)
        {
            return clientId;
        }
        public string GenerateRowKey(string assetId)
        {
            return assetId;
        }

    }
}
