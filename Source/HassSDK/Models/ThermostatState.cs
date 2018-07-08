using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HassSDK.Models
{
    public class ThermostatState : EntityRequest
    {
        [JsonProperty("temperature")]
        public int Temperature { get; set; }
    }
}
