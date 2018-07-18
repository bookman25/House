using System;
using System.Threading.Tasks;
using HassSDK;
using HouseService.Services;

namespace HouseService.AutomationBase
{
    public abstract class Automation
    {
        protected HassService HassService { get; }

        protected HassClient Client => HassService.Client;

        protected Automation(HassService hass)
        {
            HassService = hass;
        }

        public bool IsEnabled { get; set; } = true;

        public abstract Task UpdateAsync();
    }
}
