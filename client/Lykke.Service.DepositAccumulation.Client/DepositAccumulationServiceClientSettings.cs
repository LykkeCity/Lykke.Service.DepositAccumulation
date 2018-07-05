using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.DepositAccumulation.Client 
{
    /// <summary>
    /// DepositAccumulation client settings.
    /// </summary>
    public class DepositAccumulationServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
