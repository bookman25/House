using System;
using Newtonsoft.Json.Linq;

namespace HassSDK.Models
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class Field
    {
        [NotNull]
        public string Name { get; }

        public string Message { get; }

        public Field([NotNull] string name, string message)
        {
            Name = name;
            Message = message;
        }

        public static Field FromJson(JProperty json)
        {
            var name = json.Name;
            var desc = json.Value["description"].ToString();

            return new Field(Constraint.NotNull(name, "name"), desc);
        }
    }
}
