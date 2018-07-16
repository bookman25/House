using System;
using System.Collections.Immutable;
using HouseService.Devices;
using HouseService.Services;

namespace HouseService.AutomationBase
{
    public abstract class LightAutomation : Automation
    {
        protected LightAutomation(HassService hass)
            : base(hass)
        {
        }
    }
}
