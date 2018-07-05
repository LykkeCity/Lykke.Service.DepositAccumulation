using Lykke.HttpClientGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.DepositAccumulation.Client
{
    /// <summary>
    /// Deposit accumulation client interface.
    /// </summary>
    public class DepositAccumulationClient : IDepositAccumulationClient
    {
        /// <summary>
        /// Api for accumulated deposits
        /// </summary>
        public IDepositAccumulationApi DepositAccumulationApi { get; }



        /// <summary>
        /// 
        /// </summary>
        public DepositAccumulationClient(IHttpClientGenerator httpClientGenerator)
        {
            DepositAccumulationApi = httpClientGenerator.Generate<IDepositAccumulationApi>();
        }

    }
}
