using System;
using System.Threading.Tasks;
using AutoMapper;
using HassSDK.Models;
using Nest;

namespace HouseService.ElasticSearch
{
    public class DownstairsThermostatIndex : ElasticIndex
    {
        public IMapper Mapper { get; }

        public override Func<CreateIndexDescriptor, ICreateIndexRequest> IndexConfiguration => i => i
            .Settings(s => s.NumberOfShards(4).NumberOfReplicas(1))
            .Mappings(m => m.Map<ESThermostat>(map => map.AutoMap()));

        public override string IndexName { get; } = "downstairs.temperature";

        public DownstairsThermostatIndex(Lazy<ElasticSearchService> elasticSearch)
            : base(elasticSearch)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ThermostatEntity, ESThermostat>();
            });

            Mapper = config.CreateMapper();
        }

        public async override Task IndexItemAsync<T>(T state)
        {
            var item = Mapper.Map<ESThermostat>(state);
            item.Timestamp = DateTime.UtcNow;
            await ElasticSearch.Value.Client.UpdateAsync(new UpdateDescriptor<ESThermostat, object>(DocumentPath<ESThermostat>.Id(item.Id))
                .Doc(item)
                .DocAsUpsert(true)
                .Index(IndexName));
        }
    }
}
