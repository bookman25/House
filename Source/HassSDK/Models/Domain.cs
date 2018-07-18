using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json.Linq;

namespace HassSDK.Models
{
    public interface IDomain
    {
        [NotNull]
        IService GetService(string name);
    }

    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class Domain : IDomain
    {
        [NotNull]
        public string Name { get; }

        private ImmutableDictionary<string, Service> Services { get; }

        public Domain([NotNull] string name, ImmutableDictionary<string, Service> services)
        {
            Name = name;
            Services = services;
        }

        [NotNull]
        public IService GetService(string name)
        {
            return Services[name];
        }

        public static Domain FromJson(JToken json, [NotNull] HassClient client)
        {
            var name = json["domain"].ToString();
            var services = json["services"];
            var list = new List<Service>();
            foreach (var s in services)
            {
                list.Add(Service.FromJson((JProperty)s, name, client));
            }

            return new Domain(Constraint.NotNull(name, "domain"), list.ToImmutableDictionary(i => i.Name));
        }
    }
}
