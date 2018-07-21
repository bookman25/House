﻿using System;
using System.Collections.Generic;
using System.Linq;
using HouseService.Services;

namespace HouseService.ViewModels
{
    public class WebViewModel
    {
        private readonly AutomationEngine engine;

        public WebViewModel(AutomationEngine engine)
        {
            this.engine = engine;

            Automations = engine.Automations.Select(i => i.CreateViewModel()).OrderBy(i => i.Type).ToList();
        }

        public List<AutomationViewModel> Automations { get; }
    }
}
