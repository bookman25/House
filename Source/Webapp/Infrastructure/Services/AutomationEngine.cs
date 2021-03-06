﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HassSDK;
using HouseService.AutomationBase;
using HouseService.Devices;
using HouseService.ElasticSearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HouseService.Services
{
    public class AutomationEngine : IHostedService, IDisposable
    {
        public ElasticSearchService ElasticSearch { get; }

        public HassioOptions Options { get; }

        public ImmutableArray<Automation> Automations { get; }

        private HassClient Client { get; }

        private HassService HassService { get; }

        private SubscriptionClient SubscriptionClient { get; }

        private ILogger<AutomationEngine> Logger { get; }

        private Timer Timer { get; set; }

        private Computer computer;

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
            HassService = hassService;
            LogHelper.DefaultLogger = logger;

            Automations = automations.ToImmutableArray();

            computer = new Computer(HassService, EntityIds.ComputerMonitor);
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

            await Task.WhenAll(Automations.Select(i => i.LoadAsync()));
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
                using (LogHelper.PushRequestId("Main"))
                {
                    foreach (var a in Automations.Where(i => i.IsEnabled))
                    {
                        try
                        {
                            await a.UpdateAsync();
                        }
                        catch (HttpRequestException)
                        {
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, $"{a.Name} failed");
                        }
                    }
                }
            }
            finally
            {
                refreshLock.Release();
            }
        }
    }
}
