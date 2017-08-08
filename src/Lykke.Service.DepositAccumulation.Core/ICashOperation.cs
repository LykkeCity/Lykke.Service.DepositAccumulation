using System;

namespace Lykke.Service.DepositAccumulation.Core
{
    public interface ICashOperation
    {
        string Id { get; }
        string ClientId { get; }
        DateTime DateTime { get; }
        double Volume { get; }
        string Asset { get; }
    }
}
