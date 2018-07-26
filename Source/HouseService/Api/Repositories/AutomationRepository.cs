using System;
using System.Collections.Immutable;
using System.Linq;
using HouseService.Api.Models;
using HouseService.Services;

namespace HouseService.Api.Repositories
{
    public class AutomationRepository
    {
        private readonly AutomationEngine engine;

        public AutomationRepository(AutomationEngine engine)
        {
            this.engine = engine;
        }

        public ImmutableArray<AutomationViewModel> Get()
        {
            return engine.Automations.Select(i => i.CreateViewModel()).ToImmutableArray();
        }

        public AutomationViewModel Get(string id)
        {
            return Get().FirstOrDefault(i => i.Id == id);
        }
    }
}
