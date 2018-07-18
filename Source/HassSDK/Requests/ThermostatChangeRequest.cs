using System;
using Newtonsoft.Json;

namespace HassSDK.Requests
{
    public class ThermostatChangeRequest : EntityRequest
    {
        [JsonProperty("temperature")]
        public int TargetTemperature { get; }

        public ThermostatChangeRequest([NotNull] string entityId, int targetTemperature)
            : base(entityId)
        {
            TargetTemperature = targetTemperature;
        }
    }
}
