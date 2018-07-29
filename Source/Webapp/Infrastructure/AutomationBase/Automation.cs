using System;
using System.Threading.Tasks;
using HassSDK;
using HouseService.Api.Models;
using HouseService.Services;

namespace HouseService.AutomationBase
{
    public abstract class Automation
    {
        protected Automation(HassService hass)
        {
            HassService = hass;
        }

        protected HassService HassService { get; }

        protected HassClient Client => HassService.Client;

        public abstract string Id { get; }

        public abstract string Name { get; }

        public abstract AutomationType Type { get; }

        public bool IsEnabled { get; set; } = true;

        public abstract Task LoadAsync();

        public abstract Task UpdateAsync();

        public abstract AutomationViewModel CreateViewModel();
    }

    public enum AutomationType
    {
        Lights,
        Climate
    }
}
