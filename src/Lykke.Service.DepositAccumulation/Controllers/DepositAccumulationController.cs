using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.DepositAccumulation.Client.Models;
using Lykke.Service.DepositAccumulation.Core;
using Lykke.Service.DepositAccumulation.Services;
using Lykke.Service.RateCalculator.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]


    public class DepositAccumulationController
    {
        //private readonly IPaymentTransactionsRepository _paymentTransactionsRepository;
        private readonly IAccumulatedDepositRepository _accumulatedDepositRepository;
        private readonly DepositAccumulationService _depositAccumulationService;
        //private readonly IRateCalculatorClient _rateCalculatorClient;
        //private readonly ILog _log;

        public DepositAccumulationController(
            //IPaymentTransactionsRepository paymentTransactionsRepository,
            //IAccumulatedDepositRepository accumulatedDepositRepository,
            DepositAccumulationService depositAccumulationService
            //IRateCalculatorClient rateCalculatorClient
            )
        {
            //_paymentTransactionsRepository = paymentTransactionsRepository;
            //_accumulatedDepositRepository = accumulatedDepositRepository;
            _depositAccumulationService = depositAccumulationService;
            //_rateCalculatorClient = rateCalculatorClient;
        }

        [HttpGet]
        [Route("get/{clientId}")]
        public async Task<AccumulatedDepositsModel> Get(string clientId)
        {
            return await _depositAccumulationService.GetAccumulatedDepositsForUI(clientId);
        }

        /*
        [HttpPost]
        [Route("calculate/{clientId}")]
        public async Task Calculate(string clientId)
        {
            await Task.CompletedTask;
        }
        */



    }


}
