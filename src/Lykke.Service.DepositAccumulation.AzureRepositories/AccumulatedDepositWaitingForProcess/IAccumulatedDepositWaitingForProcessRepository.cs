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
        Task<IEnumerable<AccumulatedDepositWaitingForProcessEntity>> GetWaitingForProcessAsync(int generation);

        Task SaveWaitingForProcessAsync(string clientId, string transactionId, int generation);

        Task IncrementProcessingAttempt(AccumulatedDepositWaitingForProcessEntity entity);

        Task DeleteWaitingForProcessAsync(string clientId, string transactionId);

        Task MoveToNextGeneration(AccumulatedDepositWaitingForProcessEntity entity);
        

    }
}
