using AzureStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public interface IAccumulatedDepositRepository
    {
        Task<AccumulatedDepositEntity> GetAsync(string clientId, string assetId);

        Task SaveAsync(string clientId, string assetId, double amount);
    }
}
