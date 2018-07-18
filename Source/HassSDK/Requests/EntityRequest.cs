using System;
using Newtonsoft.Json;

namespace HassSDK.Requests
{
    public class EntityRequest
    {
        [JsonProperty("entity_id")]
        [NotNull]
        public string EntityId { get; }

        public EntityRequest([NotNull] string entityId)
        {
            EntityId = entityId;
        }
    }
}
