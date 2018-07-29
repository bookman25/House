using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using Elasticsearch.Net;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Serilog;

namespace HouseService.ElasticSearch
{
    public class ElasticSearchService
    {
        public ElasticClient Client { get; }

        private ImmutableArray<ElasticIndex> Indexes { get; }

        private ILogger<ElasticSearchService> Logger { get; }

        public ElasticSearchService(ILogger<ElasticSearchService> logger, IConfiguration configuration, IEnumerable<ElasticIndex> indexes)
        {
            var connectionSettings = new ConnectionSettings(configuration.GetValue<Uri>("ElasticSearch:Host"))
                .EnableDebugMode(OnRequestCompleted)
                .RequestTimeout(TimeSpan.FromSeconds(5));
            Client = new ElasticClient(connectionSettings);
            Indexes = indexes.ToImmutableArray();
            Logger = logger;
        }

        private void OnRequestCompleted(IApiCallDetails obj)
        {
            if (!obj.Success && !obj.DebugInformation.Contains("resource_already_exists_exception"))
            {
                System.Diagnostics.Debugger.Break();
                Logger.LogError(obj.DebugInformation);
            }
        }

        public async Task CreateIndexesAsync()
        {
            foreach (var index in Indexes)
            {
                try
                {
                    await Client.CreateIndexAsync(index.IndexName, index.IndexConfiguration);
                }
                catch (ElasticsearchClientException)
                {
                }
            }
        }
    }
}
