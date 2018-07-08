using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Models;

namespace HouseService.Services
{
    public class HassService
    {
        public HassClient Client { get; }

        private Temporary<Task<ImmutableDictionary<string, Domain>>> domainCache;

        public HassService(HassClient client)
        {
            Client = client;

            domainCache = new Temporary<Task<ImmutableDictionary<string, Domain>>>(() => Client.Services.GetAsync(), TimeSpan.FromMinutes(5));
        }

        public async Task<Domain> GetDomainAsync(string domain)
        {
            var domains = await domainCache.Value;
            if (domains.TryGetValue(domain, out var val))
            {
                return val;
            }

            return null;
        }
    }
}
