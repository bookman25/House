using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Threading.Tasks;
using HassSDK.Models;
using Newtonsoft.Json.Linq;

namespace HassSDK.Entities
{
    public class ServicesEntity : BaseEntity
    {
        public ServicesEntity([NotNull] HassClient client)
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
                result.Add(Domain.FromJson(item, Client));
            }

            return result.ToImmutableDictionary(i => i.Name);
        }
    }
}
