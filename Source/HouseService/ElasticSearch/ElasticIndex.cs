using System;
using System.Threading.Tasks;
using Nest;

namespace HouseService.ElasticSearch
{
    public abstract class ElasticIndex
    {
        public abstract string IndexName { get; }

        public abstract Func<CreateIndexDescriptor, ICreateIndexRequest> IndexConfiguration { get; }

        public Lazy<ElasticSearchService> ElasticSearch { get; }

        protected ElasticIndex(Lazy<ElasticSearchService> elasticSearch)
        {
            ElasticSearch = elasticSearch;
        }

        public abstract Task IndexItemAsync<T>(T state)
            where T : class;
    }
}
