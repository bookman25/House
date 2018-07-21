using System;
using System.Collections.Immutable;
using System.Linq;
using HouseService.AutomationBase;
using HouseService.Devices;

namespace HouseService.ViewModels
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
                Status = "Lights are on";
            }
            else
            {
                Status = "Lights are off";
            }
        }

        public ImmutableArray<LightGroup> Lights { get; }

        public override string Icon => "light.svg";
    }
}
