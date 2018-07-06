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
    public class AccumulatedDepositRepository : IAccumulatedDepositRepository
    {
        private readonly INoSQLTableStorage<AccumulatedDepositEntity> _tableStorage;
        private readonly INoSQLTableStorage<AccumulatedDepositPeriodEntity> _periodTableStorage;

        public AccumulatedDepositRepository(
            INoSQLTableStorage<AccumulatedDepositEntity> tableStorage,
            INoSQLTableStorage<AccumulatedDepositPeriodEntity> periodTableStorage,
            ILog log
            )
        {
            _tableStorage = tableStorage;
            _periodTableStorage = periodTableStorage;
        }

        public async Task<AccumulatedDepositEntity> GetAsync(string clientId, string assetId)
        {
             return await _tableStorage.GetDataAsync(GeneratePartition(clientId), GenerateRowKey(assetId));
        }

        public async Task SavePeriodAsync(IAccumulatedDepositPeriod period)
        {
            var partitionKey = period.ClientId;
            var rowKey = GeneratePeriodKey(period);

            IAccumulatedDepositPeriod existingPeriod = await _periodTableStorage.GetDataAsync(partitionKey, rowKey);
            if (existingPeriod == null)
            {
                var entity = new AccumulatedDepositPeriodEntity
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,

                    ClientId = period.ClientId,
                    AssetId = period.AssetId,
                    Amount = period.Amount,
                    AmountInUsd = period.AmountInUsd,
                    StartDateTime = period.StartDateTime
                };
                await _periodTableStorage.InsertAsync(entity);
            }
            else
            {
                await _periodTableStorage.MergeAsync(partitionKey, rowKey, rowData => {
                    rowData.Amount = Math.Round(rowData.Amount + period.Amount, 15);
                    rowData.AmountInUsd = Math.Round(rowData.AmountInUsd + period.AmountInUsd, 15);
                    return rowData;
                });
            }
        }

        public async Task SaveAssetTotalAsync(IAccumulatedDepositPeriod period)
        {
            var partitionKey = period.ClientId;
            var rowKey = $"__Total-{period.AssetId}__";

            IAccumulatedDepositPeriod existingPeriod = await _periodTableStorage.GetDataAsync(partitionKey, rowKey);
            if (existingPeriod == null)
            {
                var entity = new AccumulatedDepositPeriodEntity
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,

                    ClientId = period.ClientId,
                    AssetId = period.AssetId,
                    Amount = period.Amount,
                    AmountInUsd = period.AmountInUsd,
                    StartDateTime = period.StartDateTime
                };

                await _periodTableStorage.InsertAsync(entity);
            }
            else
            {
                await _periodTableStorage.MergeAsync(partitionKey, rowKey, rowData => {
                    rowData.Amount = Math.Round(rowData.Amount + period.Amount, 15);
                    rowData.AmountInUsd = Math.Round(rowData.AmountInUsd + period.AmountInUsd, 15);
                    return rowData;
                });
            }
        }

        public async Task SaveTotalAsync(IAccumulatedDepositPeriod period)
        {
            var partitionKey = period.ClientId;
            var rowKey = $"__Total__";

            IAccumulatedDepositPeriod existingPeriod = await _periodTableStorage.GetDataAsync(partitionKey, rowKey);
            if (existingPeriod == null)
            {
                var entity = new AccumulatedDepositPeriodEntity
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,

                    ClientId = period.ClientId,
                    AssetId = "USD",
                    Amount = period.AmountInUsd,
                    AmountInUsd = period.AmountInUsd,
                    StartDateTime = period.StartDateTime
                };

                await _periodTableStorage.InsertAsync(entity);
            }
            else
            {
                await _periodTableStorage.MergeAsync(partitionKey, rowKey, rowData => {
                    rowData.Amount = rowData.AmountInUsd = Math.Round(rowData.AmountInUsd + period.AmountInUsd, 15);
                    return rowData;
                });
            }
        }

        public string GeneratePeriodKey(IAccumulatedDepositPeriod period)
        {
            return String.Format($"1d-{{0:yyyyMMdd}}-{period.AssetId}", period.StartDateTime);
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

        public async Task<double> GetForDayAsync(string clientId, DateTime dt)
        {
            var rowKey = String.Format($"1d-{{0:yyyyMMdd}}", dt);
            var entity = await _tableStorage.GetDataAsync(clientId, rowKey);
            return entity == null ? 0 : entity.Amount;
        }

        public async Task<double> GetForAllTimeAsync(string clientId)
        {
            var entity = await _tableStorage.GetDataAsync(clientId, "AllTime");
            return entity == null ? 0 : entity.Amount;
        }



    }
}
