using System;
using Newtonsoft.Json;

namespace HassSDK.Requests
{
    public class ThermostatChangeRequest : EntityRequest
    {
        [JsonProperty("temperature")]
        public int TargetTemperature { get; set; }
    }
}
