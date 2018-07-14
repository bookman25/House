using System;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Models;
using HassSDK.Requests;
using HouseService.Devices;
using HouseService.Services;

namespace HouseService.AutomationBase
{
    public abstract class LightAutomation : Automation
    {
        protected LightGroup Light { get; }

        protected LightAutomation(HassService hass, string entityId)
            : base(hass)
        {
            Light = new LightGroup(hass, entityId);
        }

        protected Task TurnOffAsync()
        {
            return Light.TurnOffAsync(10);
        }

        protected Task TurnOnAsync(int? brightness = null, int? blueLevel = null, int? transition = null)
        {
            return Light.TurnOnAsync(new LightChangeRequest { Brightness = brightness, ColorTemp = BlueToColorTemp(blueLevel), Transition = transition });
        }

        protected async Task ChangeLevelsAsync(int? brightness = null, int? blueLevel = null, int? transition = null)
        {
            await Light.ChangeLevelsAsync(new LightChangeRequest { Brightness = brightness, ColorTemp = BlueToColorTemp(blueLevel), Transition = transition });
        }

        private int? BlueToColorTemp(int? bluePercent)
        {
            if (bluePercent == null)
            {
                return null;
            }

            bluePercent = 100 - bluePercent;
            var blue = 153;
            var orange = 500;

            var range = orange - blue;
            var newValue = range * bluePercent / 100f;
            return blue + (int)newValue;
        }
    }
}
