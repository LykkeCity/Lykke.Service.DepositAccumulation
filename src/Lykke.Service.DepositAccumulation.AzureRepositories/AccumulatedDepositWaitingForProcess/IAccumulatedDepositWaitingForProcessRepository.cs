using AzureStorage;
using Lykke.Service.DepositAccumulation.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public interface IAccumulatedDepositWaitingForProcessRepository
    {
        Task<IEnumerable<AccumulatedDepositWaitingForProcessEntity>> GetWaitingForProcessAsync(string clientId);

        Task SaveWaitingForProcessAsync(string clientId, string transactionId);

        Task DeleteWaitingForProcessAsync(string clientId, string transactionId);

    }
}
