using System;
using System.Threading.Tasks;
using HouseService.AutomationBase;
using HouseService.Devices;
using HouseService.Services;

namespace HouseService.Automations
{
    public class LivingRoomLights : LightAutomation
    {
        private readonly HueLight light;

        public LivingRoomLights(HassService hass)
            : base(hass)
        {
            light = new HueLight(hass, EntityIds.LivingRoomLight);
        }

        public override Task UpdateAsync()
        {
            //return light.ChangeColorAsync("salmon", brightness: 128);
            return Task.CompletedTask;
        }
    }
}
