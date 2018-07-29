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
    public class LightsController : Controller
    {
        private readonly AutomationEngine engine;

        public LightsController(AutomationEngine engine)
        {
            this.engine = engine;
        }

        [HttpPost("Set({id})")]
        public async Task<AutomationViewModel> Set(string id)
        {
            var turnOn = HttpContext.Request.Query["turnOn"].First();
            var automation = engine.Automations.First(i => i.Id == id);
            if (automation is LightAutomation lights)
            {
                foreach (var light in lights.Lights)
                {
                    if (bool.Parse(turnOn))
                    {
                        await light.TurnOnAsync();
                    }
                    else
                    {
                        await light.TurnOffAsync();
                    }
                }
            }

            return automation.CreateViewModel();
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
