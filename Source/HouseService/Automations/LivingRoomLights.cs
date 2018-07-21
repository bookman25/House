using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using HouseService.AutomationBase;
using HouseService.Devices;
using HouseService.Extensions;
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
            IsEnabled = false;
            Lights = ImmutableArray<LightGroup>.Empty.Add(light);
        }

        public override string Name => "Living Room Lights";

        public override async Task UpdateAsync()
        {
            var random = new Random();
            await light.ChangeColorAsync(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), brightness: 128);

            if (DateTime.Now.DayOfWeek > DayOfWeek.Sunday && DateTime.Now.DayOfWeek < DayOfWeek.Saturday)
            {
                if (DateTime.Now.IsAfter("11:15am") && DateTime.Now.IsBefore("12:45pm"))
                {
                    await light.TurnOnAsync();
                }
                else
                {
                    await light.TurnOffAsync();
                }
            }
        }
    }
}
