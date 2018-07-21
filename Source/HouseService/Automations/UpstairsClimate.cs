using System;
using HouseService.AutomationBase;
using HouseService.ElasticSearch;
using HouseService.Extensions;
using HouseService.Services;
using Microsoft.Extensions.Logging;

namespace HouseService.Automations
{
    public class UpstairsClimate : ClimateAutomation
    {
        public override string Name { get; } = "Upstairs Climate";

        public UpstairsClimate(HassService hass, SensorService sensors, UpstairsThermostatIndex index, ILogger<UpstairsClimate> logger)
            : base(hass, EntityIds.UpstairsThermostatCooling, sensors.UpstairsThermostat, index, logger)
        {
        }

        protected override int GetTimeBasedTargetTemperature()
        {
            var date = DateTime.Now;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                    if (date.IsBefore("3:00am"))
                    {
                        return 74;
                    }
                    else if (date.IsBefore("4:00am"))
                    {
                        return 75;
                    }
                    else if (date.IsBefore("6:00am"))
                    {
                        return 78;
                    }
                    else if (date.IsBefore("8:30pm"))
                    {
                        return 80;
                    }
                    else
                    {
                        return 74;
                    }
                case DayOfWeek.Friday:
                    if (date.IsBefore("3:00am"))
                    {
                        return 74;
                    }
                    else if (date.IsBefore("6:00am"))
                    {
                        return 78;
                    }
                    else
                    {
                        return 80;
                    }
                case DayOfWeek.Saturday:
                    return 80;
                case DayOfWeek.Sunday:
                    if (date.IsBefore("8:30pm"))
                    {
                        return 80;
                    }
                    else
                    {
                        return 73;
                    }
                default:
                    throw new InvalidOperationException("Unknown DayOfWeek");
            }
        }
    }
}
