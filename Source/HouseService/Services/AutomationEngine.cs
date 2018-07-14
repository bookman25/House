﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HassSDK;
using HouseService.AutomationBase;
using HouseService.ElasticSearch;
using HouseService.Sensors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HouseService.Services
{
    public class AutomationEngine : IHostedService, IDisposable
    {
        private HassClient Client { get; }

        public ElasticSearchService ElasticSearch { get; }

        public HassioOptions Options { get; }

        private HassService HassService { get; }

        private SubscriptionClient SubscriptionClient { get; }

        private ILogger<AutomationEngine> Logger { get; }

        private Timer Timer { get; set; }

        private ImmutableArray<Automation> Automations { get; }

        public ImmutableDictionary<string, Sensor> Sensors { get; } = ImmutableDictionary<string, Sensor>.Empty;

        public AutomationEngine(
            HassClient client,
            HassService hassService,
            ElasticSearchService elasticSearch,
            SubscriptionClient subscriptionClient,
            IConfiguration configuration,
            IEnumerable<Automation> automations,
            ILogger<AutomationEngine> logger)
        {
            Client = client;
            ElasticSearch = elasticSearch;
            Options = configuration.GetSection("Hassio").Get<HassioOptions>();
            SubscriptionClient = subscriptionClient;
            Logger = logger;
            HassService = HassService;

            Automations = automations.ToImmutableArray();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var login = await Client.AuthenticateAsync(Options.Endpoint, Options.Password);
            if (!login)
            {
                throw new InvalidOperationException("Not logged in!");
            }

            Logger.LogInformation("Starting engine...");
            await ElasticSearch.CreateIndexesAsync();

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
                await Task.WhenAll(Automations.Select(i => i.UpdateAsync()));
            }
            finally
            {
                refreshLock.Release();
            }
        }
    }
}
