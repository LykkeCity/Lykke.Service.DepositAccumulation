using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.DepositAccumulation.Core.Domain
{
    public interface IAccumulatedDepositPeriod
    {
        DateTime StartDateTime { get; set; }

        string ClientId { get; set; }
        string AssetId { get; set; }
        double? Amount { get; set; }
        double AmountInUsd { get; set; }
    }
}



