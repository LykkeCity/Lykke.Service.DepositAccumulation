﻿using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.DepositAccumulation.Core;
using Lykke.Service.DepositAccumulation.Models;
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
        //private readonly IAccumulatedDepositRepository _accumulatedDepositRepository;
        private readonly DepositAccumulationCalculationService _depositAccumulationCalculationService;
        private readonly IRateCalculatorClient _rateCalculatorClient;
        //private readonly ILog _log;

        public DepositAccumulationController(
            //IPaymentTransactionsRepository paymentTransactionsRepository,
            //IAccumulatedDepositRepository accumulatedDepositRepository,
            DepositAccumulationCalculationService depositAccumulationCalculationService,
            IRateCalculatorClient rateCalculatorClient,
            RabbitMqSubscriber<CashTransferOperation> vvv
            )
        {
            //_paymentTransactionsRepository = paymentTransactionsRepository;
            //_accumulatedDepositRepository = accumulatedDepositRepository;
            _depositAccumulationCalculationService = depositAccumulationCalculationService;
            _rateCalculatorClient = rateCalculatorClient;
        }

        [HttpPost]
        [Route("get")]
        public async Task<DepositAccumulationResponse> Get([FromBody] DepositAccumulationRequest request)
        {
            DepositAccumulationResponse resp = new DepositAccumulationResponse();
            /*
            resp.AmountCHF = await DepositAccumulationService.Get(_paymentTransactionsRepository, _accumulatedDepositRepository, request.ClientId, "CHF");
            resp.AmountEUR = await DepositAccumulationService.Get(_paymentTransactionsRepository, _accumulatedDepositRepository, request.ClientId, "EUR");
            resp.AmountGBP = await DepositAccumulationService.Get(_paymentTransactionsRepository, _accumulatedDepositRepository, request.ClientId, "GBP");
            resp.AmountUSD = await DepositAccumulationService.Get(_paymentTransactionsRepository, _accumulatedDepositRepository, request.ClientId, "USD");
            */
            return resp;
        }

        [HttpPost]
        [Route("calculate/{clientId}")]
        public async Task Calculate(string clientId)
        {
            await Task.CompletedTask;
        }

    }


}
