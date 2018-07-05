using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.DepositAccumulation.Core.Domain
{
    public class AccumulatedDepositPeriod : IAccumulatedDepositPeriod
    {
        public DateTime StartDateTime { get; set; }

        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public double Amount { get; set; }
        public double AmountInUsd { get; set; }
    }
}
