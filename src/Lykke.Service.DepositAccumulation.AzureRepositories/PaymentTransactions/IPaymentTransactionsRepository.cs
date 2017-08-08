using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.AzureRepositories
{
    public interface IPaymentTransactionsRepository
    {
        Task<double> SumAllAsync(string clientId, string assetId);
    }
}
