using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.DepositAccumulation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public class PaymentTransactionsRepository : IPaymentTransactionsRepository
    {
        public static readonly string NotifyProcessedStatus = "NotifyProcessed";
        private readonly INoSQLTableStorage<PaymentTransactionEntity> _tableStorage;

        public PaymentTransactionsRepository(INoSQLTableStorage<PaymentTransactionEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        /*
        public async Task<double> SumAllAsync(string clientId, string assetId)
        {
            var allPaymentTransactions = await _tableStorage.GetDataAsync(GeneratePartition(clientId), _ => _.AssetId == assetId);
            double result = 0;
            if (allPaymentTransactions != null)
            {
                foreach (PaymentTransactionEntity e in allPaymentTransactions)
                {
                    if (e.Status == NotifyProcessedStatus)
                    {
                        result += e.Amount;
                    }
                }
            }
            return result;
        }
        */

        private Func<PaymentTransactionEntity, bool> FiatPredicate = (PaymentTransactionEntity t) => (t.AssetId == "CHF" || t.AssetId == "USD" || t.AssetId == "EUR" || t.AssetId == "GBP") && t.Status == NotifyProcessedStatus;

        public async Task<IEnumerable<PaymentTransactionEntity>> GetAllFiatAsync(string clientId)
        {
            return (await _tableStorage.GetDataAsync(GeneratePartition(clientId))).Where(FiatPredicate);
        }

        public async Task<PaymentTransactionEntity> GetTransactionAsync(string clientId, string transactionId)
        {
            return await _tableStorage.GetDataAsync(GeneratePartition(clientId), transactionId);
        }

        public string GeneratePartition(string clientId)
        {
            return clientId;
        }


    }
}
