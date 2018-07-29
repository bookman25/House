using System;
using System.Collections.Immutable;
using System.Linq;
using HouseService.AutomationBase;
using HouseService.Devices;

namespace HouseService.Api.Models
{
    public class LightAutomationViewModel : AutomationViewModel
    {
        public LightAutomationViewModel(Automation automation)
            : base(automation)
        {
            var lightAutomation = (LightAutomation)automation;
            Lights = lightAutomation.Lights;

            if (Lights.Any(i => i.IsOn))
            {
                IsOn = true;
                Status = "Lights are on";
            }
            else
            {
                IsOn = false;
                Status = "Lights are off";
            }
        }

        public bool IsOn { get; set; }

        public ImmutableArray<LightGroup> Lights { get; }

        public override string Icon => "light.svg";
    }
}
