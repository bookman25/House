using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Models;
using HouseService.AutomationBase;
using HouseService.Automations;
using HouseService.DeviceTypes;
using HouseService.Sensors;
using Microsoft.Extensions.Hosting;

namespace HouseService.Services
{
    public class AutomationEngine : IHostedService, IDisposable
    {
        private HassClient Client { get; }

        private HassService HassService { get; }

        private SubscriptionClient SubscriptionClient { get; }

        private Timer Timer { get; set; }

        private List<Automation> Services { get; }

        public ImmutableDictionary<string, Sensor> Sensors { get; } = ImmutableDictionary<string, Sensor>.Empty;

        public AutomationEngine(HassioOptions options)
        {
            Client = new HassClient(options.Endpoint, options.Password);
            SubscriptionClient = new SubscriptionClient(Client);

            HassService = new HassService(Client);
            Services = new List<Automation>()
            {
                new UpstairsClimate(HassService),
                new KitchenLights(HassService, SubscriptionClient)
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var login = await Client.AuthenticateAsync();
            if (!login)
            {
                throw new InvalidOperationException("Not logged in!");
            }

            await SubscriptionClient.StartAsync();
            Timer = new Timer(Refresh, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await SubscriptionClient.StopAsync();
            Timer.Change(Timeout.Infinite, 0);
        }

        public void Dispose()
        {
            Timer?.Dispose();
        }

        private SemaphoreSlim refreshLock = new SemaphoreSlim(1);

        private async void Refresh(object state)
        {
            if (!(await refreshLock.WaitAsync(0)))
            {
                return;
            }

            try
            {
                await Task.WhenAll(Services.Select(i => i.UpdateAsync()));
            }
            finally
            {
                refreshLock.Release();
            }
        }
    }
}
