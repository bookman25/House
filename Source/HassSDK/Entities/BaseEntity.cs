using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HassSDK.Entities
{
    public abstract class BaseEntity
    {
        protected HassClient Client { get; }

        public BaseEntity(HassClient client)
        {
            Client = client;
        }

        protected async Task<T> HandleResponseAsync<T>(HttpResponseMessage message)
            where T : class
        {
            var content = message.Content == null ? null : await message.Content.ReadAsStringAsync();
            if (content == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
