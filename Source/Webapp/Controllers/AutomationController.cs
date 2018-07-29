using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseService.Api.Models;
using HouseService.Api.Repositories;
using HouseService.AutomationBase;
using HouseService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Webapp.Controllers
{
    [Route("api/[controller]")]
    public class AutomationController : Controller
    {
        private readonly AutomationEngine engine;
        private readonly AutomationRepository automationRepo;

        public AutomationController(AutomationEngine engine, AutomationRepository automationRepo)
        {
            this.engine = engine;
            this.automationRepo = automationRepo;
        }

        // GET: api/Automation
        [HttpGet]
        public IEnumerable<AutomationViewModel> Get()
        {
            return automationRepo.Get();
        }

        // GET: api/Automation/5
        [HttpGet("{id}", Name = "Get")]
        public AutomationViewModel Get(string id)
        {
            return automationRepo.Get(id);
        }

        [HttpPost("Toggle({id})")]
        public async Task Toggle(string id)
        {
            var automation = engine.Automations.First(i => i.Id == id);
            if (automation is LightAutomation lights)
            {
                foreach (var light in lights.Lights)
                {
                    await light.ToggleAsync();
                }
            }
        }
    }
}
