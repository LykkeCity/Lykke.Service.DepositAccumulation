using AzureStorage;
using Lykke.Service.DepositAccumulation.Core.Domain;
using System;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public class AccumulatedDepositRepository : IAccumulatedDepositRepository
    {
        private readonly INoSQLTableStorage<AccumulatedDepositPeriodEntity> _tableStorage;

        public AccumulatedDepositRepository(
            INoSQLTableStorage<AccumulatedDepositPeriodEntity> tableStorage
            )
        {
            _tableStorage = tableStorage;
        }

        public async Task SavePeriodAsync(AccumulatedDepositPeriodEntity period)
        {
            IAccumulatedDepositPeriod existingPeriod = await _tableStorage.GetDataAsync(period.PartitionKey, period.RowKey);
            if (existingPeriod == null)
            {
                await _tableStorage.InsertAsync(period);
            }
            else
            {
                await _tableStorage.MergeAsync(period.PartitionKey, period.RowKey, rowData => {
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

            IAccumulatedDepositPeriod existingPeriod = await _tableStorage.GetDataAsync(partitionKey, rowKey);
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

                await _tableStorage.InsertAsync(entity);
            }
            else
            {
                await _tableStorage.MergeAsync(partitionKey, rowKey, rowData => {
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

            IAccumulatedDepositPeriod existingPeriod = await _tableStorage.GetDataAsync(partitionKey, rowKey);
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

                await _tableStorage.InsertAsync(entity);
            }
            else
            {
                await _tableStorage.MergeAsync(partitionKey, rowKey, rowData => {
                    rowData.Amount = rowData.AmountInUsd = Math.Round(rowData.AmountInUsd + period.AmountInUsd, 15);
                    return rowData;
                });
            }
        }

        public string GeneratePeriodKey(IAccumulatedDepositPeriod period)
        {
            return String.Format($"1d-{{0:yyyyMMdd}}-{period.AssetId}", period.StartDateTime);
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
