using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.DepositAccumulation.Models;
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
        private readonly IPaymentTransactionsRepository _paymentTransactionsRepository;
        private readonly IAccumulatedDepositRepository _accumulatedDepositRepository;

        public DepositAccumulationController(
            IPaymentTransactionsRepository paymentTransactionsRepository,
            IAccumulatedDepositRepository accumulatedDepositRepository
            )
        {
            _paymentTransactionsRepository = paymentTransactionsRepository;
            _accumulatedDepositRepository = accumulatedDepositRepository;

        }

        [HttpPost]
        [Route("get")]
        public async Task<DepositAccumulationResponse> Get([FromBody] DepositAccumulationRequest request)
        {
            DepositAccumulationResponse resp = new DepositAccumulationResponse();
            resp.AmountCHF = await DepositAccumulationService.GetAmountOfAccumulatedDeposits(_paymentTransactionsRepository, _accumulatedDepositRepository, request.ClientId, "CHF");
            resp.AmountEUR = await DepositAccumulationService.GetAmountOfAccumulatedDeposits(_paymentTransactionsRepository, _accumulatedDepositRepository, request.ClientId, "EUR");
            resp.AmountGBP = await DepositAccumulationService.GetAmountOfAccumulatedDeposits(_paymentTransactionsRepository, _accumulatedDepositRepository, request.ClientId, "GBP");
            resp.AmountUSD = await DepositAccumulationService.GetAmountOfAccumulatedDeposits(_paymentTransactionsRepository, _accumulatedDepositRepository, request.ClientId, "USD");
            return resp;
        }
    }


}
