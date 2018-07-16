using System;
using System.Threading.Tasks;
using HassSDK;
using HouseService.AutomationBase;
using HouseService.Devices;
using HouseService.Extensions;
using HouseService.Services;

namespace HouseService.Automations
{
    public class KitchenLights : LightAutomation
    {
        private readonly FluxLight kitchenLight;

        private readonly FluxLight pantryLight;

        public KitchenLights(HassService hass, SubscriptionClient subscriptionClient, SensorService sensors)
            : base(hass)
        {
            kitchenLight = new FluxLight(hass, EntityIds.KitchenLight);
            kitchenLight.AddMotionSensor(sensors.KitchenMotionSensor);

            pantryLight = new FluxLight(hass, EntityIds.PantryLight);
            pantryLight.AddMotionSensor(sensors.KitchenMotionSensor);
        }

        public override async Task UpdateAsync()
        {
            var kitchenLevels = GetTimeBasedLevels(kitchenLight);
            await kitchenLight.ChangeLevelsAsync(kitchenLevels.brightness, kitchenLevels.blueLevel, kitchenLevels.transitionTime);
            var pantryLevels = GetTimeBasedLevels(pantryLight);
            await pantryLight.ChangeLevelsAsync(pantryLevels.brightness, pantryLevels.blueLevel, pantryLevels.transitionTime);
        }

        private (int? brightness, int? blueLevel, int? transitionTime) GetTimeBasedLevels(LightGroup light)
        {
            var levels = GetTimeBasedLevels();
            if (light.EntityId == EntityIds.PantryLight)
            {
                levels.brightness = levels.brightness / 2;
            }

            return levels;
        }

        private (int? brightness, int? blueLevel, int? transitionTime) GetTimeBasedLevels()
        {
            int transitionTime = 5;
            var date = DateTime.Now;
            if (date.IsBefore("5:00am"))
            {
                return (brightness: 64, blueLevel: 0, transitionTime);
            }
            else if (date.IsBefore("10:00am"))
            {
                return (brightness: 255, blueLevel: 50, transitionTime);
            }
            else if (date.IsBefore("4:00pm"))
            {
                return (brightness: 255, blueLevel: 100, transitionTime);
            }
            else if (date.IsBefore("6:00pm"))
            {
                return (brightness: 255, blueLevel: 100, transitionTime);
            }
            else if (date.IsBefore("7:00pm"))
            {
                return (brightness: 240, blueLevel: 80, transitionTime);
            }
            else if (date.IsBefore("7:45pm"))
            {
                return (brightness: 230, blueLevel: 60, transitionTime);
            }
            else if (date.IsBefore("9:00pm"))
            {
                return (brightness: 128, blueLevel: 40, transitionTime);
            }
            else if (date.IsBefore("10:00pm"))
            {
                return (brightness: 128, blueLevel: 20, transitionTime);
            }
            else
            {
                return (brightness: 32, blueLevel: 0, transitionTime);
            }
        }
    }
}
