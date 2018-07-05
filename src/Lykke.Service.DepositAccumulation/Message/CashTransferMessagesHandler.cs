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
            //IAccumulatedDepositWaitingForProcessRepository accumulatedDepositWaitingForProcessRepository,
            ILogFactory log)
        {
            _rmqSettings = rmqSettings;
            //_accumulatedDepositWaitingForProcessRepository = accumulatedDepositWaitingForProcessRepository;
            _log = log;
            
            /*
            _subscriber = subscriber;
            _subscriber.SetMessageDeserializer(new JsonMessageDeserializer<CashTransferOperation>())
                .SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy())
                .Subscribe(HandleMessage)
                .SetLogger(log);
                */

        }

        public async Task StartAsync()
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
                //_log.WriteErrorAsync(nameof(CashInOutQueue), nameof(Start), null, ex).Wait();
                throw;
            }


            await Task.CompletedTask;
        }

        private async Task<bool> HandleMessage(CashTransferOperation item)
        {
            try
            {
                //await _accumulatedDepositWaitingForProcessRepository.SaveWaitingForProcessAsync(item.ClientId, item.Id);

                return await Task.FromResult(true);
            }
            catch (Exception exc)
            {
                await _log.CreateLog(this).WriteErrorAsync(
                    nameof(DepositAccumulation),
                    nameof(CashTransferMessagesHandler),
                    nameof(HandleMessage),
                    exc.GetBaseException(),
                    DateTime.UtcNow);

                return await Task.FromResult(false);
            }
        }

    }
}
