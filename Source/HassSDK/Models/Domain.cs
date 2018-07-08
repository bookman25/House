using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json.Linq;

namespace HassSDK.Models
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class Domain
    {
        [NotNull]
        public string Name { get; }

        public ImmutableDictionary<string, Service> Services { get; }

        public Domain([NotNull] string name, ImmutableDictionary<string, Service> services)
        {
            Name = name;
            Services = services;
        }

        public static Domain FromJson(JToken json)
        {
            var name = json["domain"].ToString();
            var services = json["services"];
            var list = new List<Service>();
            foreach (var s in services)
            {
                list.Add(Service.FromJson((JProperty)s, name));
            }

            return new Domain(Constraint.NotNull(name, "domain"), list.ToImmutableDictionary(i => i.Name));
        }
    }
}
