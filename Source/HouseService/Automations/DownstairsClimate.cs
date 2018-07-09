using System;
using System.Threading.Tasks;
using HassSDK;
using HouseService.AutomationBase;
using HouseService.Extensions;
using HouseService.Services;

namespace HouseService.Automations
{
    public class DownstairsClimate : ClimateAutomation
    {
        public DownstairsClimate(HassService hass, SensorService sensors)
            : base(hass, EntityIds.DownstairsThermostatCooling, sensors.DownstairsThermostat)
        {
        }

        public override async Task UpdateAsync()
        {
            var date = DateTime.Now;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                case DayOfWeek.Friday:
                    if (date.IsBefore("6:00am"))
                    {
                        await SetTemperatureAsync(80);
                    }
                    if (date.IsBefore("6:40am"))
                    {
                        await SetTemperatureAsync(78);
                    }
                    if (date.IsBefore("11:00am"))
                    {
                        await SetTemperatureAsync(80);
                    }
                    else if (date.IsBefore("12:30pm"))
                    {
                        await SetTemperatureAsync(78);
                    }
                    else if (date.IsBefore("3:45pm"))
                    {
                        await SetTemperatureAsync(80);
                    }
                    else if (date.IsBefore("9:30pm"))
                    {
                        await SetTemperatureAsync(77);
                    }
                    else
                    {
                        if (date.DayOfWeek == DayOfWeek.Friday)
                        {
                            await SetTemperatureAsync(76);
                        }
                        else
                        {
                            await SetTemperatureAsync(80);
                        }
                    }
                    break;
                case DayOfWeek.Saturday:
                    if (date.IsBefore("10:00pm"))
                    {
                        await SetTemperatureAsync(77);
                    }
                    else
                    {
                        await SetTemperatureAsync(76);
                    }
                    break;
                case DayOfWeek.Sunday:
                    if (date.IsBefore("10:00pm"))
                    {
                        await SetTemperatureAsync(77);
                    }
                    else
                    {
                        await SetTemperatureAsync(80);
                    }
                    break;
            }
        }
    }
}
