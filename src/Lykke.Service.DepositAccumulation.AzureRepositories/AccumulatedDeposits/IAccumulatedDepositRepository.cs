using AzureStorage;
using Lykke.Service.DepositAccumulation.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public interface IAccumulatedDepositRepository
    {
        Task<AccumulatedDepositEntity> GetAsync(string clientId, string assetId);

        Task<double> GetForDayAsync(string clientId, DateTime dt);

        Task<double> GetForAllTimeAsync(string clientId);

        Task SaveAsync(string clientId, string assetId, double amount);

        Task SavePeriodAsync(IAccumulatedDepositPeriod period);

        Task SaveAssetTotalAsync(IAccumulatedDepositPeriod period);

        Task SaveTotalAsync(IAccumulatedDepositPeriod period);

    }
}
