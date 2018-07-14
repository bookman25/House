using System;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HassSDK.Models
{
    public abstract class Entity
    {
        public string EntityId { get; set; }

        public DateTime LastChanged { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? NodeId { get; set; }

        public string FriendlyName { get; set; }

        public string State { get; set; }

        public ImmutableDictionary<string, object> Attributes { get; set; }

        public T GetAttribute<T>(string key)
        {
            return (T)Attributes[key];
        }

        protected virtual void SetProperties(JToken json)
        {
            EntityId = json["entity_id"].Value<string>();
            State = json["state"].Value<string>();
            LastChanged = json["last_changed"].Value<DateTime>();
            LastUpdated = json["last_updated"].Value<DateTime>();

            var builder = ImmutableDictionary.CreateBuilder<string, object>();
            foreach (var property in json["attributes"].Cast<JProperty>())
            {
                switch (property.Value)
                {
                    case JValue val:
                        builder.Add(property.Name, ((JValue)property.Value).Value);
                        break;
                    case JArray array:
                        builder.Add(property.Name, array);
                        break;
                    default:
                        throw new NotImplementedException($"Json value type not handled: {property.Value.GetType()}");
                }
            }

            Attributes = builder.ToImmutable();
            FriendlyName = (string)builder["friendly_name"];
            Attributes.TryGetValue("node_id", out var nodeId);
            NodeId = nodeId as int?;
        }

        public static GenericEntity CreateGenericEntity(JToken json)
        {
            var entity = new GenericEntity();
            entity.SetProperties(json);
            return entity;
        }

        public static T FromJson<T>(JToken json)
            where T : Entity, new()
        {
            var entity = new T();
            entity.SetProperties(json);
            return entity;
        }
    }

    public class GenericEntity : Entity
    {
    }
}
