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
    public class StatesEntity : BaseEntity
    {
        public StatesEntity([NotNull] HassClient client)
            : base(client)
        {
        }

        public async Task<ImmutableDictionary<string, GenericEntity>> GetAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Client.BaseUri + "/states");
            var response = await Client.HttpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var jobject = JArray.Parse(content);

            var result = new List<GenericEntity>();
            foreach (var item in jobject)
            {
                result.Add(Entity.CreateGenericEntity(item));
            }

            return result.ToImmutableDictionary(i => i.EntityId);
        }

        public async Task<GenericEntity> GetEntityAsync(string entityId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Client.BaseUri + "/states/" + entityId);
            var response = await Client.HttpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var jobject = JObject.Parse(content);
            return Entity.CreateGenericEntity(jobject);
        }

        public async Task<T> GetEntityAsync<T>(string entityId)
            where T : Entity, new()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Client.BaseUri + "/states/" + entityId);
            var response = await Client.HttpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var jobject = JObject.Parse(content);
            return Entity.FromJson<T>(jobject);
        }
    }
}
