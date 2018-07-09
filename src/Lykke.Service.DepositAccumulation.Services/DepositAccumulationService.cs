using Lykke.Service.DepositAccumulation.AzureRepositories;
using Lykke.Service.DepositAccumulation.Client.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.DepositAccumulation.Services
{
    public class DepositAccumulationService
    {
        private readonly IAccumulatedDepositRepository _accumulatedDepositRepository;

        public DepositAccumulationService(
            IAccumulatedDepositRepository accumulatedDepositRepository
            )
        {
            _accumulatedDepositRepository = accumulatedDepositRepository;
        }

        public async Task<AccumulatedDepositsModel> GetAccumulatedDepositsForUI(string clientId)
        {
            DateTime now = DateTime.UtcNow;

            AccumulatedDepositsModel resp = new AccumulatedDepositsModel();
            resp.AmountTotal = await _accumulatedDepositRepository.GetForAllTimeAsync(clientId);
            resp.Amount1Day = await _accumulatedDepositRepository.GetForDayAsync(clientId, now);

            DateTime reportDt = now;
            List<Task<double>> tasks = new List<Task<double>>();
            for (int i = 0; i < 30; i++)
            {
                Task<double> t = _accumulatedDepositRepository.GetForDayAsync(clientId, reportDt);
                tasks.Add(t);

                reportDt = reportDt.AddDays(-1);
            }
            await Task.WhenAll(tasks.ToArray());

            tasks.ForEach(t => resp.Amount30Days += t.Result);

            return resp;
        }


    }
}
