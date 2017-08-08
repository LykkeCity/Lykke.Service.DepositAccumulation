using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.DepositAccumulation.Core;
using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.DepositAccumulation.Services;

namespace Lykke.Service.DepositAccumulation.Message
{
    public class SeparatingMessagesHandler
    {
        private readonly RabbitMqSubscriber<CashTransferOperation> _subscriber;
        private readonly IPaymentTransactionsRepository _paymentTransactionsRepository;
        private readonly IAccumulatedDepositRepository _accumulatedDepositRepository;
        private readonly ILog _log;
        private DateTime _lastInvalidWarning = DateTime.MinValue;
        private DateTime _lastExecuteWarning = DateTime.MinValue;

        public SeparatingMessagesHandler(
            RabbitMqSubscriber<CashTransferOperation> subscriber,
            IPaymentTransactionsRepository paymentTransactionsRepository,
            IAccumulatedDepositRepository accumulatedDepositRepository,
            ILog log)
        {
            _paymentTransactionsRepository = paymentTransactionsRepository;
            _accumulatedDepositRepository = accumulatedDepositRepository;
            _log = log;
            _subscriber = subscriber;
            _subscriber.SetMessageDeserializer(new JsonMessageDeserializer<CashTransferOperation>())
                .SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy())
                .Subscribe(HandleMessage)
                .SetLogger(log);

        }

        private async Task<bool> HandleMessage(CashTransferOperation item)
        {
            try
            {
                double amount = await DepositAccumulationService.GetAmountOfAccumulatedDeposits(_paymentTransactionsRepository, _accumulatedDepositRepository, item.ClientId, item.Asset);
                await _accumulatedDepositRepository.SaveAsync(item.ClientId, item.Asset, amount + item.Volume);

                return await Task.FromResult(true);
            }
            catch (Exception exc)
            {
                await _log.WriteErrorAsync(
                    nameof(DepositAccumulation),
                    nameof(SeparatingMessagesHandler),
                    nameof(HandleMessage),
                    exc.GetBaseException(),
                    DateTime.UtcNow);

                return await Task.FromResult(false);
            }
        }

    }
}
