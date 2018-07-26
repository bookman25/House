using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseService.Api.Models;
using HouseService.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HouseService.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Automation")]
    public class AutomationController : Controller
    {
        private readonly AutomationRepository automationRepo;

        public AutomationController(AutomationRepository automationRepo)
        {
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
    }
}
