using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HassSDK.Requests
{
    public class EntityRequest
    {
        [JsonProperty("entity_id")]
        public string EntityId { get; set; }
    }
}
