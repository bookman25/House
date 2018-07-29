using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using HouseService.Api.Models;
using HouseService.Devices;
using HouseService.Services;

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

        public override async Task LoadAsync()
        {
            foreach (var light in Lights)
            {
                await light.LoadAsync();
            }
        }

        public override AutomationViewModel CreateViewModel()
        {
            return new LightAutomationViewModel(this);
        }
    }
}
