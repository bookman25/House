using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using Elasticsearch.Net;

using Microsoft.Extensions.Configuration;

using Nest;
using Serilog;

namespace HouseService.ElasticSearch
{
    public class ElasticSearchService
    {
        public ElasticClient Client { get; }

        private ImmutableArray<ElasticIndex> Indexes { get; }

        public ElasticSearchService(IConfiguration configuration, IEnumerable<ElasticIndex> indexes)
        {
            var connectionSettings = new ConnectionSettings(configuration.GetValue<Uri>("ElasticSearch:Host"))
                .EnableDebugMode(OnRequestCompleted)
                .RequestTimeout(TimeSpan.FromSeconds(5));
            Client = new ElasticClient(connectionSettings);
            Indexes = indexes.ToImmutableArray();
        }

        private void OnRequestCompleted(IApiCallDetails obj)
        {
            if (!obj.Success && !obj.DebugInformation.Contains("resource_already_exists_exception"))
            {
                System.Diagnostics.Debugger.Break();
                Log.Error(obj.DebugInformation);
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
