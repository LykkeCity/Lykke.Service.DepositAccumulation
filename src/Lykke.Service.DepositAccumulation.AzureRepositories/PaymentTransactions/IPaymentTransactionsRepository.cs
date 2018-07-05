using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public interface IPaymentTransactionsRepository
    {
        Task<double> SumAllAsync(string clientId, string assetId);

        Task<IEnumerable<PaymentTransactionEntity>> GetAllFiatAsync(string clientId);

        Task<PaymentTransactionEntity> GetTransactionAsync(string clientId, string transactionId);
    }
}
