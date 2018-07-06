using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.DepositAccumulation.Core;
using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.DepositAccumulation.Services;
using Lykke.Common.Log;
using Autofac;
using Lykke.RabbitMqBroker;

namespace Lykke.Service.DepositAccumulation.Message
{
    public class CashTransferMessagesHandler
    {
        private RabbitMqSubscriber<CashTransferOperation> _subscriber;

        private readonly RabbitMqSubscriptionSettings _rmqSettings;
        private readonly IAccumulatedDepositWaitingForProcessRepository _accumulatedDepositWaitingForProcessRepository;
        private readonly ILogFactory _log;

        public CashTransferMessagesHandler(
            RabbitMqSubscriptionSettings rmqSettings,
            IAccumulatedDepositWaitingForProcessRepository accumulatedDepositWaitingForProcessRepository,
            ILogFactory log)
        {
            _rmqSettings = rmqSettings;
            _accumulatedDepositWaitingForProcessRepository = accumulatedDepositWaitingForProcessRepository;
            _log = log;
        }

        public void Start()
        {
            try
            {
                _subscriber = new RabbitMqSubscriber<CashTransferOperation>(_log, _rmqSettings, new DeadQueueErrorHandlingStrategy(_log, _rmqSettings))
                    .SetMessageDeserializer(new JsonMessageDeserializer<CashTransferOperation>())
                    .SetMessageReadStrategy(new MessageReadQueueStrategy())
                    .Subscribe(HandleMessage)
                    .CreateDefaultBinding()
                    .Start();
            }
            catch (Exception ex)
            {
                _log.CreateLog(this).Error(ex, nameof(CashTransferMessagesHandler), nameof(Start));
            }
        }

        private async Task HandleMessage(CashTransferOperation item)
        {
            try
            {
                if (item.Asset == "CHF" || item.Asset == "USD" || item.Asset == "EUR" || item.Asset == "GBP")
                {
                    await _accumulatedDepositWaitingForProcessRepository.SaveWaitingForProcessAsync(item.ClientId, item.Id, 0);
                }
            }
            catch (Exception ex)
            {
                _log.CreateLog(this).Error(ex, nameof(CashTransferMessagesHandler), nameof(HandleMessage));
            }
        }

    }
}
