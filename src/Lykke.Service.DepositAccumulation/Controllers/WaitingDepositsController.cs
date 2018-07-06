using Common;
using Lykke.Common.Log;
using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.DepositAccumulation.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.Controllers
{
    public class WaitingDepositsController : TimerPeriod
    {
        private readonly DepositAccumulationProcessingService _depositAccumulationProcessingService;

        public WaitingDepositsController(
            DepositAccumulationProcessingService depositAccumulationProcessingService,
            ILogFactory _log
            ) : base(TimeSpan.FromSeconds(5), _log, nameof(WaitingDepositsController))
        {
            _depositAccumulationProcessingService = depositAccumulationProcessingService;
        }

        public override async Task Execute()
        {
            await _depositAccumulationProcessingService.ProcessWaitingRecords();
        }
    }


}
