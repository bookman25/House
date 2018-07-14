using System;
using System.Threading.Tasks;
using HassSDK;
using HouseService.AutomationBase;
using HouseService.ElasticSearch;
using HouseService.Extensions;
using HouseService.Services;

namespace HouseService.Automations
{
    public class UpstairsClimate : ClimateAutomation
    {
        public UpstairsClimate(HassService hass, SensorService sensors, UpstairsThermostatIndex index)
            : base(hass, EntityIds.UpstairsThermostatCooling, sensors.UpstairsThermostat, index)
        {
        }

        public override async Task UpdateAsync()
        {
            await base.UpdateAsync();

            var date = DateTime.Now;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                    if (date.IsBefore("3:00am"))
                    {
                        await SetTemperatureAsync(74);
                    }
                    else if (date.IsBefore("4:00am"))
                    {
                        await SetTemperatureAsync(75);
                    }
                    else if (date.IsBefore("6:00am"))
                    {
                        await SetTemperatureAsync(78);
                    }
                    else if (date.IsBefore("8:30pm"))
                    {
                        await SetTemperatureAsync(80);
                    }
                    else
                    {
                        await SetTemperatureAsync(74);
                    }
                    break;
                case DayOfWeek.Friday:
                    if (date.IsBefore("3:00am"))
                    {
                        await SetTemperatureAsync(74);
                    }
                    else if (date.IsBefore("6:00am"))
                    {
                        await SetTemperatureAsync(78);
                    }
                    else
                    {
                        await SetTemperatureAsync(80);
                    }
                    break;
                case DayOfWeek.Saturday:
                    await SetTemperatureAsync(80);
                    break;
                case DayOfWeek.Sunday:
                    if (date.IsBefore("8:30pm"))
                    {
                        await SetTemperatureAsync(80);
                    }
                    else
                    {
                        await SetTemperatureAsync(73);
                    }
                    break;
            }
        }
    }
}
