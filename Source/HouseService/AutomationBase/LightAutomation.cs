using System;
using System.Collections.Immutable;
using HouseService.Devices;
using HouseService.Services;
using HouseService.ViewModels;

namespace HouseService.AutomationBase
{
    public abstract class LightAutomation : Automation
    {
        public override AutomationType Type => AutomationType.Lights;

        public ImmutableArray<LightGroup> Lights { get; set; }

        protected LightAutomation(HassService hass)
            : base(hass)
        {
        }

        public override AutomationViewModel CreateViewModel()
        {
            return new LightAutomationViewModel(this);
        }
    }
}
