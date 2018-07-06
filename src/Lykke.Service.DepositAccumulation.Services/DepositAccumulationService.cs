using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.DepositAccumulation.Client.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.Services
{
    public class DepositAccumulationService
    {
        private readonly IAccumulatedDepositRepository _accumulatedDepositRepository;

        public DepositAccumulationService(
            IAccumulatedDepositRepository accumulatedDepositRepository
            )
        {
            _accumulatedDepositRepository = accumulatedDepositRepository;
        }

        public async Task<AccumulatedDepositsModel> GetAccumulatedDepositsForUI(string clientId)
        {
            AccumulatedDepositsModel resp = new AccumulatedDepositsModel();
            resp.AmountTotal = await _accumulatedDepositRepository.GetForAllTimeAsync(clientId);
            return resp;
        }


        /*
        public static async Task<double> AccumulateDepositsAndAddAmount(
            IPaymentTransactionsRepository paymentTransactionsRepository,
            IAccumulatedDepositRepository accumulatedDepositRepository,
            string clientId,
            string assetId,
            double addAmount
            )
        {
            var accumulatedDeposit = await accumulatedDepositRepository.GetAsync(clientId, assetId);
            double amount = 0;
            if (accumulatedDeposit == null)
            {
                // new amount is already in DB - so it is already summarized
                amount = await paymentTransactionsRepository.SumAllAsync(clientId, assetId);
            }
            else
            {
                amount = accumulatedDeposit.Amount + addAmount;
            }

            await accumulatedDepositRepository.SaveAsync(clientId, assetId, amount);

            return amount;
        }

        public static async Task<double> Get(
            IPaymentTransactionsRepository paymentTransactionsRepository,
            IAccumulatedDepositRepository accumulatedDepositRepository,
            string clientId,
            string assetId
            )
        {
            var accumulatedDeposit = await accumulatedDepositRepository.GetAsync(clientId, assetId);
            if (accumulatedDeposit == null) // not summarized yet
            {
                double amount = await paymentTransactionsRepository.SumAllAsync(clientId, assetId);
                await accumulatedDepositRepository.SaveAsync(clientId, assetId, amount);
                return amount;
            }
            else
            {
                return accumulatedDeposit.Amount;
            }

        }
        */

    }
}
