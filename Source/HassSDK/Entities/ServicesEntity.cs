using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HassSDK.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassSDK.Entities
{
    public class ServicesEntity : BaseEntity
    {
        public ServicesEntity(HassClient client)
            : base(client)
        {
        }

        public async Task<ImmutableDictionary<string, Domain>> GetAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Client.BaseUri + "/services");
            var response = await Client.HttpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var jobject = JArray.Parse(content);

            var result = new List<Domain>();
            foreach (var item in jobject)
            {
                result.Add(Domain.FromJson(item));
            }

            return result.ToImmutableDictionary(i => i.Name);
        }

        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        public async Task<bool> CallServiceAsync(Service service, object payload)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Client.BaseUri + service.Endpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(payload, jsonSettings), Encoding.UTF8, "application/json");
            var response = await Client.HttpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
