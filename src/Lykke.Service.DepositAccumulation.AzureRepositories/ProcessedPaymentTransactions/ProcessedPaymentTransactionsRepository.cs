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
    public class ProcessedPaymentTransactionsRepository : IProcessedPaymentTransactionsRepository
    {
        private readonly INoSQLTableStorage<ProcessedPaymentTransactionEntity> _tableStorage;

        public ProcessedPaymentTransactionsRepository(INoSQLTableStorage<ProcessedPaymentTransactionEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task SaveAsync(PaymentTransactionEntity pt)
        {
            ProcessedPaymentTransactionEntity ppt = new ProcessedPaymentTransactionEntity
            {
                PartitionKey = pt.ClientId,
                RowKey = pt.TransactionId,

                Created = pt.Created,
                TransactionId = pt.TransactionId,
                PaymentSystem = pt.PaymentSystem,
                Status = pt.Status,

                ClientId = pt.ClientId,
                AssetId = pt.AssetId,
                Amount = pt.Amount,
                AmountInUsd = pt.AmountInUsd,
                ExchangeRate = pt.ExchangeRate
            };

            await _tableStorage.InsertOrReplaceAsync(ppt);
        }

        public async Task<bool> IsInReposioryAsync(string clientId, string transactionId)
        {
            return await _tableStorage.RecordExistsAsync(
                new ProcessedPaymentTransactionEntity
                {
                    PartitionKey = clientId,
                    RowKey = transactionId,
                });
        }


    }
}
