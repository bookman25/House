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

        public SubscriptionClient SubscriptionClient { get; }

        private Temporary<Task<ImmutableDictionary<string, Domain>>> domainCache;

        public HassService(HassClient client, SubscriptionClient subscriptionClient)
        {
            Client = client;
            SubscriptionClient = subscriptionClient;

            domainCache = new Temporary<Task<ImmutableDictionary<string, Domain>>>(() => Client.Services.GetAsync(), TimeSpan.FromMinutes(5));
        }

        public void Subscribe(string entityId, Action<EventData> action)
        {
            SubscriptionClient.Subscribe(entityId, e =>
            {
                using (LogHelper.PushRequestId("Subscription"))
                {
                    action(e);
                }
            });
        }

        public async Task<IDomain> GetDomainAsync(string domain)
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
