using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json.Linq;

namespace HassSDK.Models
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class Service
    {
        [NotNull]
        public string Name { get; }

        [NotNull]
        public string Domain { get; }

        public string Description { get; }

        public ImmutableArray<Field> Fields { get; }

        public string Endpoint => $"/services/{Domain}/{Name}";

        public Service([NotNull] string domain, [NotNull] string name, string description, ImmutableArray<Field> fields)
        {
            Domain = domain;
            Name = name;
            Description = description;
            Fields = fields;
        }

        public static Service FromJson(JProperty json, string domain)
        {
            var name = json.Name;
            var desc = json.Value["description"].ToString();
            var fields = new List<Field>();
            foreach (var field in json.Value["fields"])
            {
                var p = (JProperty)field;
                if (p.Value.Type == JTokenType.String)
                {
                    break;
                }
                fields.Add(Field.FromJson(p));
            }

            return new Service(Constraint.NotNull(domain, "domain"), Constraint.NotNull(name, "name"), desc, fields.ToImmutableArray());
        }
    }
}
