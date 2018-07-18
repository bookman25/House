using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HassSDK.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassSDK.Models
{
    public interface IService
    {
        Task<bool> ExecuteAsync(EntityRequest request);
    }

    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class Service : IService
    {
        [NotNull]
        public string Name { get; }

        [NotNull]
        public string Domain { get; }

        public string Description { get; }

        public ImmutableArray<Field> Fields { get; }

        public string Endpoint => $"/services/{Domain}/{Name}";

        private readonly HassClient client;

        public Service([NotNull] HassClient client, [NotNull] string domain, [NotNull] string name, string description, ImmutableArray<Field> fields)
        {
            this.client = client;

            Domain = domain;
            Name = name;
            Description = description;
            Fields = fields;
        }

        public async Task<bool> ExecuteAsync(EntityRequest request)
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, client.BaseUri + Endpoint);
            msg.Content = new StringContent(JsonConvert.SerializeObject(request, client.JsonSettings), Encoding.UTF8, "application/json");
            var response = await client.HttpClient.SendAsync(msg);
            return response.IsSuccessStatusCode;
        }

        public static Service FromJson(JProperty json, string domain, [NotNull] HassClient client)
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

            return new Service(client, Constraint.NotNull(domain, "domain"), Constraint.NotNull(name, "name"), desc, fields.ToImmutableArray());
        }
    }
}
