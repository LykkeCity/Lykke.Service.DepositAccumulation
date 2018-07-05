using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public interface IProcessedPaymentTransactionsRepository
    {
        Task SaveAsync(PaymentTransactionEntity pt);
        Task<bool> IsInReposioryAsync(string clientId, string transactionId);
    }
}
