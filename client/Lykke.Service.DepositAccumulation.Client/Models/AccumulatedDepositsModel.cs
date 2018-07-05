using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.DepositAccumulation.Client.Models
{
    /// <summary>
    /// Accumulated deposits amount model
    /// </summary>
    public class AccumulatedDepositsModel
    {
        /// <summary>
        /// Accumulated deposits amount for all time
        /// </summary>
        public double AmountTotal { get; set; }
        /// <summary>
        /// Accumulated deposits amount for last day
        /// </summary>
        public double Amount1Day { get; set; }
        /// <summary>
        /// Accumulated deposits amount for latest 30 days
        /// </summary>
        public double Amount30Days { get; set; }
    }
}
