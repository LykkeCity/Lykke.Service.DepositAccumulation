using Lykke.Service.DepositAccumulation.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.DepositAccumulation.Services
{
    public class DepositAccumulationPeriodService
    {
        /*
        private int GetPeriodNumber(DateTime start, DateTime target, int daysInPeriod)
        {
            if (target < start) {
                throw new ArgumentOutOfRangeException("target", $"target DateTime {target} cannot be lesser than start DateTime {start}");
            }
            double periodNum = (target - start).TotalDays / daysInPeriod;
            var res = Convert.ToInt32(Math.Truncate(periodNum));
            return res;
        }

        private IAccumulatedDepositPeriod GeneratePeriod(DateTime start, int periodNumber, int daysInPeriod)
        {
            return new AccumulatedDepositPeriod
            {
                StartDateTime = start.AddDays(daysInPeriod * periodNumber),
                EndDateTime = start.AddDays(daysInPeriod * (periodNumber + 1)).AddTicks(-1),
                DaysInPeriod = daysInPeriod
            };
        }

        public IAccumulatedDepositPeriod GenerateDayPeriod(DateTime start, DateTime target)
        {
            int pn = GetPeriodNumber(start, target, 1);
            return GeneratePeriod(start, pn, 1);
        }
        */


    }


}
