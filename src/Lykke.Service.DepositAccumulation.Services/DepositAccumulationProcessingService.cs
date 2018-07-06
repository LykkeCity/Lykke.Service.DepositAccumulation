using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.RateCalculator.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.Services
{
    public class DepositAccumulationProcessingService
    {
        private readonly int maxProcessAttempts = 50;

        private readonly IAccumulatedDepositRepository _accumulatedDepositRepository;
        private readonly IPaymentTransactionsRepository _paymentTransactionsRepository;
        private readonly IProcessedPaymentTransactionsRepository _processedPaymentTransactionsRepository;
        private readonly IAccumulatedDepositWaitingForProcessRepository _accumulatedDepositWaitingForProcessRepository;

        private readonly IRateCalculatorClient _rateCalculatorClient;


        public DepositAccumulationProcessingService(
            IAccumulatedDepositRepository accumulatedDepositRepository,
            IPaymentTransactionsRepository paymentTransactionsRepository,
            IProcessedPaymentTransactionsRepository processedPaymentTransactionsRepository,
            IAccumulatedDepositWaitingForProcessRepository accumulatedDepositWaitingForProcessRepository,

            IRateCalculatorClient rateCalculatorClient
            )
        {
            _accumulatedDepositRepository = accumulatedDepositRepository;
            _paymentTransactionsRepository = paymentTransactionsRepository;
            _processedPaymentTransactionsRepository = processedPaymentTransactionsRepository;
            _accumulatedDepositWaitingForProcessRepository = accumulatedDepositWaitingForProcessRepository;

            _rateCalculatorClient = rateCalculatorClient;
        }

        public async Task ProcessWaitingRecords()
        {
            IEnumerable<AccumulatedDepositWaitingForProcessEntity> gen0 = await _accumulatedDepositWaitingForProcessRepository.GetWaitingForProcessAsync(0);
            foreach (var entity in gen0)
            {
                var keys = entity.RowKey.Split("|");
                var clientId = keys[0];
                var transactionId = keys[1];

                var transaction = await _paymentTransactionsRepository.GetTransactionAsync(clientId, transactionId);
                if (transaction != null && transaction.Status == PaymentTransactionsRepository.NotifyProcessedStatus)
                {
                    await ProcessTransaction(transaction);

                    await _processedPaymentTransactionsRepository.SaveAsync(transaction);
                    await _accumulatedDepositWaitingForProcessRepository.DeleteWaitingForProcessAsync(entity.PartitionKey, entity.RowKey);
                }
                else
                {
                    if (entity.Generation0ProcessAttempts >= maxProcessAttempts)
                    {
                        string prevPartitionKey = entity.PartitionKey;
                        await _accumulatedDepositWaitingForProcessRepository.MoveToNextGeneration(entity);
                        await _accumulatedDepositWaitingForProcessRepository.DeleteWaitingForProcessAsync(prevPartitionKey, entity.RowKey);
                    }
                    else
                    {
                        await _accumulatedDepositWaitingForProcessRepository.IncrementProcessingAttempt(entity);
                    }
                }
            }
        }

        public async Task ProcessTransaction(PaymentTransactionEntity transaction)
        {
            string clientId = transaction.ClientId;
            string assetId = transaction.AssetId;
            double amount = transaction.Amount;
            DateTime startDateTime = transaction.Created.Date;

            double rate = 1;

            if (transaction.AssetId != "USD")
            {
                var mp = await _rateCalculatorClient.GetMarketProfileAsync();
                rate = await _rateCalculatorClient.GetAmountInBaseWithProfileAsync("USD", 1, transaction.AssetId, mp);
            }

            double amountInUsd = transaction.Amount * rate;


            transaction.AmountInUsd = amountInUsd;
            transaction.ExchangeRate = rate;
            await _processedPaymentTransactionsRepository.SaveAsync(transaction);


            // one day period
            {
                var rowKey = String.Format($"1d-{{0:yyyyMMdd}}-{assetId}", startDateTime);
                var entity = new AccumulatedDepositPeriodEntity
                {
                    PartitionKey = clientId,
                    RowKey = rowKey,

                    ClientId = clientId,
                    AssetId = assetId,
                    Amount = amount,
                    AmountInUsd = amountInUsd,
                    StartDateTime = startDateTime
                };
                await _accumulatedDepositRepository.SavePeriodAsync(entity);
            }

            // asset total accumulated period
            {
                var rowKey = $"AllTime-{transaction.AssetId}";
                var entity = new AccumulatedDepositPeriodEntity
                {
                    PartitionKey = clientId,
                    RowKey = rowKey,

                    ClientId = clientId,
                    AssetId = assetId,
                    Amount = amount,
                    AmountInUsd = amountInUsd,
                    StartDateTime = startDateTime
                };

                await _accumulatedDepositRepository.SavePeriodAsync(entity);
            }

            // one day total accumulated period in USD
            {
                var rowKey = String.Format($"1d-{{0:yyyyMMdd}}", startDateTime);
                var entity = new AccumulatedDepositPeriodEntity
                {
                    PartitionKey = clientId,
                    RowKey = rowKey,

                    ClientId = clientId,
                    AssetId = "USD",
                    Amount = amount,
                    AmountInUsd = amountInUsd,
                    StartDateTime = startDateTime
                };
                await _accumulatedDepositRepository.SavePeriodAsync(entity);
            }

            // all time total accumulated period in USD
            {
                var rowKey = $"AllTime";
                var entity = new AccumulatedDepositPeriodEntity
                {
                    PartitionKey = clientId,
                    RowKey = rowKey,

                    ClientId = clientId,
                    AssetId = "USD",
                    Amount = amount,
                    AmountInUsd = amountInUsd,
                    StartDateTime = startDateTime
                };
                await _accumulatedDepositRepository.SavePeriodAsync(entity);
            }

        }

    }




}
