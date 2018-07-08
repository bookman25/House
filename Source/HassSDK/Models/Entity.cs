using System;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HassSDK.Models
{
    public class Entity
    {
        [NotNull]
        public string EntityId { get; }

        public int? NodeId { get; }

        public string FriendlyName { get; }

        public string State { get; }

        public DateTime LastChanged { get; }

        public DateTime LastUpdated { get; }

        private readonly ImmutableDictionary<string, object> attributes;

        public Entity(
            [NotNull] string entityId,
            int? nodeId,
            string friendlyName,
            string state,
            DateTime lastChanged,
            DateTime lastUpdated,
            JToken attributes)
        {
            EntityId = entityId;
            NodeId = nodeId;
            FriendlyName = friendlyName;
            State = state;
            LastChanged = lastChanged;
            LastUpdated = lastUpdated;

            this.attributes = attributes.OfType<JProperty>().ToImmutableDictionary(i => i.Name, i => i.Value.ToObject<object>());
        }

        public T GetAttribute<T>(string key)
        {
            return (T)attributes[key];
        }

        public static Entity FromJson(JToken json)
        {
            var attr = json["attributes"];
            var nodeId = attr.Value<int>("node_id");
            var name = attr.Value<string>("friendly_name");
            var id = json["entity_id"].ToObject<string>();
            var lastChanged = json["last_changed"].ToObject<DateTime>();
            var lastUpdated = json["last_updated"].ToObject<DateTime>();
            var state = json["state"].ToObject<string>();

            return new Entity(Constraint.NotNull(id, "entityId"), nodeId, name, state, lastChanged, lastUpdated, attr);
        }
    }
}
