using Lykke.Service.DepositAccumulation.AzureRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.Services
{
    public class DepositAccumulationService
    {
        public static async Task<double> GetAmountOfAccumulatedDeposits(
            IPaymentTransactionsRepository paymentTransactionsRepository,
            IAccumulatedDepositRepository accumulatedDepositRepository,
            string clientId,
            string assetId
            )
        {
            var accumulatedDeposit = await accumulatedDepositRepository.GetAsync(clientId, assetId);
            double amount = 0;
            if (accumulatedDeposit == null)
            {
                amount = await paymentTransactionsRepository.SumAllAsync(clientId, assetId);
            }
            else
            {
                amount = accumulatedDeposit.Amount;
            }

            return amount;
        }
    }
}
