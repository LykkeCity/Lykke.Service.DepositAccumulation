﻿using System;

namespace Lykke.Service.DepositAccumulation.Core
{
    public class CashTransferOperation : ICashOperation
    {
        public string Id { get; set; }

        public string FromClientId { get; set; }

        public string ToClientId { get; set; }

        public DateTime DateTime { get; set; }

        public double Volume { get; set; }

        public string Asset { get; set; }

        public string ClientId => ToClientId;
    }
}