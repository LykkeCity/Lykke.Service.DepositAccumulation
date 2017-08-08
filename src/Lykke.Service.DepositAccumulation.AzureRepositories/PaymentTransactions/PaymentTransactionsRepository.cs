using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.DepositAccumulation.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public class PaymentTransactionsRepository : IPaymentTransactionsRepository
    {
        private readonly INoSQLTableStorage<PaymentTransactionEntity> _tableStorage;

        public PaymentTransactionsRepository(AppSettings appSettings, ILog log)
        {
            _tableStorage = new AzureTableStorage<PaymentTransactionEntity>(appSettings.DepositAccumulationService.Db.PaymentTransactionsConnectionString, "PaymentTransactions", log);
        }


        public async Task<double> SumAllAsync(string clientId, string assetId)
        {
            var allPaymentTransactions = await _tableStorage.GetDataAsync(GeneratePartition(clientId));
            double result = 0;
            if (allPaymentTransactions != null)
            {
                foreach (PaymentTransactionEntity e in allPaymentTransactions)
                {
                    result += e.Amount;
                }
            }
            return result;
        }

        public string GeneratePartition(string clientId)
        {
            return clientId;
        }


    }
}
