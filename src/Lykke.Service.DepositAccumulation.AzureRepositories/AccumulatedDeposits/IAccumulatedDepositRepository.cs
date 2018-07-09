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
        Task<double> GetForDayAsync(string clientId, DateTime dt);

        Task<double> GetForAllTimeAsync(string clientId);

        Task SavePeriodAsync(AccumulatedDepositPeriodEntity period);


    }
}
