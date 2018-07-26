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
            Lights = ImmutableArray<LightGroup>.Empty.Add(light);
        }

        public override string Name => "Living Room Lights";

        public override string Id => "livingroom.lights";

        public override async Task UpdateAsync()
        {
            //var random = new Random();
            //await light.ChangeColorAsync(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), brightness: 128);

            if (DateTime.Now.DayOfWeek >= DayOfWeek.Sunday && DateTime.Now.DayOfWeek < DayOfWeek.Friday)
            {
                if (DateTime.Now.IsAfter("10:00pm"))
                {
                    await light.TurnOffAsync();
                }
                if (DateTime.Now.IsAfter("6:25am") && DateTime.Now.IsBefore("7:00am"))
                {
                    await light.TurnOnAsync();
                }
                else if (DateTime.Now.IsAfter("7:00am") && DateTime.Now.IsBefore("7:01am"))
                {
                    await light.TurnOffAsync();
                }
            }
        }
    }
}
