using Lykke.Service.DepositAccumulation.Client.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.Client
{
    /// <summary>
    /// Accumulated deposits service
    /// </summary>
    public interface IDepositAccumulationApi
    {
        /// <summary>
        /// Gets accumulated deposits amounts
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [Get("/api/depositaccumulation/{clientId}")]
        Task<AccumulatedDepositsModel> GetAccumulatedDepositsAsync(string clientId);

    }
}
