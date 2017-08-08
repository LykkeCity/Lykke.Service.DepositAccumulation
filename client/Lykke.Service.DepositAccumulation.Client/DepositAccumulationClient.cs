using System;
using Common.Log;

namespace Lykke.Service.DepositAccumulation.Client
{
    public class DepositAccumulationClient : IDepositAccumulationClient, IDisposable
    {
        private readonly ILog _log;

        public DepositAccumulationClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
