using System;
using System.Threading.Tasks;
using HassSDK;
using HouseService.AutomationBase;
using HouseService.Extensions;
using HouseService.Sensors;
using HouseService.Services;

namespace HouseService.Automations
{
    public class KitchenLights : LightAutomation
    {
        public KitchenLights(HassService hass, SubscriptionClient subscriptionClient)
            : base(hass, "light.kitchen")
        {
            Light.AddMotionSensor(new MotionSensor(subscriptionClient, "sensor.aeotec_zw100_multisensor_6_burglar"));
        }

        public override async Task UpdateAsync()
        {
            int transitionTime = 5;
            var date = DateTime.Now;
            if (date.IsBefore("5:00am"))
            {
                await ChangeLevelsAsync(64, 0, transitionTime);
            }
            else if (date.IsBefore("10:00am"))
            {
                await ChangeLevelsAsync(255, 50, transitionTime);
            }
            else if (date.IsBefore("4:00pm"))
            {
                await ChangeLevelsAsync(255, 100, transitionTime);
            }
            else if (date.IsBefore("6:00pm"))
            {
                await ChangeLevelsAsync(255, 100, transitionTime);
            }
            else if (date.IsBefore("7:00pm"))
            {
                await ChangeLevelsAsync(240, 80, transitionTime);
            }
            else if (date.IsBefore("7:45pm"))
            {
                await ChangeLevelsAsync(230, 60, transitionTime);
            }
            else if (date.IsBefore("9:00pm"))
            {
                await ChangeLevelsAsync(128, 40, transitionTime);
            }
            else if (date.IsBefore("10:00pm"))
            {
                await ChangeLevelsAsync(128, 20, transitionTime);
            }
            else
            {
                await ChangeLevelsAsync(32, 0, transitionTime);
            }
        }
    }
}
