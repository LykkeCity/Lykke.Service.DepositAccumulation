using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.DepositAccumulation.Client.Models;
using Lykke.Service.DepositAccumulation.Core.Domain;
using Lykke.Service.RateCalculator.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.Services
{
    public class DepositAccumulationCalculationService
    {
        private readonly IAccumulatedDepositRepository _accumulatedDepositRepository;


        public DepositAccumulationCalculationService(
            IAccumulatedDepositRepository accumulatedDepositRepository
            )
        {
            _accumulatedDepositRepository = accumulatedDepositRepository;
        }



    }




}
