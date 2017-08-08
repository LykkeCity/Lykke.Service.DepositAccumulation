using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.DepositAccumulation.Models
{
    public class DepositAccumulationRequest
    {
        public string ClientId { get; set; }
        public string AssetId { get; set; }
    }
}
