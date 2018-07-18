using System;
using System.Net.Http;
using System.Threading.Tasks;
using HassSDK.Entities;
using Newtonsoft.Json;

namespace HassSDK
{
    public class HassClient
    {
        internal HttpClient HttpClient { get; }

        public ServicesEntity Services { get; }

        public StatesEntity States { get; }

        internal string BaseUri { get; private set; }

        internal string Password { get; private set; }

        internal readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        public HassClient()
        {
            HttpClient = new HttpClient();
            Services = new ServicesEntity(this);
            States = new StatesEntity(this);
        }

        public async Task<bool> AuthenticateAsync(string baseUri, string password = null)
        {
            BaseUri = baseUri;
            Password = password ?? Password;
            HttpClient.DefaultRequestHeaders.Add("x-ha-access", Password);
            var response = await HttpClient.GetAsync(BaseUri + "/");
            return response.IsSuccessStatusCode;
        }
    }
}
