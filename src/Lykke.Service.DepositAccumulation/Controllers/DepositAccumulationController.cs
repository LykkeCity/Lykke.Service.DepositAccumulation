using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.DepositAccumulation.Client.Models;
using Lykke.Service.DepositAccumulation.Core;
using Lykke.Service.DepositAccumulation.Services;
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
        private readonly DepositAccumulationService _depositAccumulationService;

        public DepositAccumulationController(
            DepositAccumulationService depositAccumulationService
            )
        {
            _depositAccumulationService = depositAccumulationService;
        }

        [HttpGet]
        [Route("get/{clientId}")]
        public async Task<AccumulatedDepositsModel> Get(string clientId)
        {
            return await _depositAccumulationService.GetAccumulatedDepositsForUI(clientId);
        }

    }


}
