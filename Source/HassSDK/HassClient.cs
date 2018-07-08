using System;
using System.Net.Http;
using System.Threading.Tasks;
using HassSDK.Entities;

namespace HassSDK
{
    public class HassClient
    {
        internal HttpClient HttpClient { get; }

        public ServicesEntity Services { get; }

        public StatesEntity States { get; }

        internal string BaseUri { get; }

        internal string Password { get; private set; }

        public HassClient(string baseUri, string password = null)
        {
            BaseUri = baseUri;
            Password = password;
            HttpClient = new HttpClient();
            Services = new ServicesEntity(this);
            States = new StatesEntity(this);
        }

        public async Task<bool> AuthenticateAsync(string password = null)
        {
            Password = password ?? Password;
            HttpClient.DefaultRequestHeaders.Add("x-ha-access", Password);
            var response = await HttpClient.GetAsync(BaseUri + "/");
            return response.IsSuccessStatusCode;
        }
    }
}
