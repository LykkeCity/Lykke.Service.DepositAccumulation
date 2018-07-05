using JetBrains.Annotations;

namespace Lykke.Service.DepositAccumulation.Client
{
    /// <summary>
    /// Deposit accumulation client interface.
    /// </summary>
    [PublicAPI]
    public interface IDepositAccumulationClient
    {
        /// <summary>
        /// Api for accumulated deposits
        /// </summary>
        IDepositAccumulationApi DepositAccumulationApi { get; }
    }
}
