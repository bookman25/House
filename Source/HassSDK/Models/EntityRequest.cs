using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HassSDK.Models
{
    public class EntityRequest
    {
        [JsonProperty("entity_id")]
        public string EntityId { get; set; }
    }
}
