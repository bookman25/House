using System;
using System.Threading.Tasks;
using HouseService.AutomationBase;
using HouseService.ElasticSearch;
using HouseService.Extensions;
using HouseService.Services;
using Microsoft.Extensions.Logging;

namespace HouseService.Automations
{
    public class DownstairsClimate : ClimateAutomation
    {
        public override string Name => "Downstairs Climate";

        public override string Id => "downstairs.climate";

        public DownstairsClimate(HassService hass, SensorService sensors, DownstairsThermostatIndex index, ILogger<DownstairsClimate> logger)
            : base(hass, EntityIds.DownstairsThermostatCooling, sensors.DownstairsThermostat, index, logger)
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
                case DayOfWeek.Friday:
                    if (date.IsBefore("6:00am"))
                    {
                        return 80;
                    }
                    if (date.IsBefore("6:40am"))
                    {
                        return 78;
                    }
                    if (date.IsBefore("11:00am"))
                    {
                        return 80;
                    }
                    else if (date.IsBefore("12:30pm"))
                    {
                        return 78;
                    }
                    else if (date.IsBefore("3:45pm"))
                    {
                        return 80;
                    }
                    else if (date.IsBefore("9:30pm"))
                    {
                        return 77;
                    }
                    else
                    {
                        if (date.DayOfWeek == DayOfWeek.Friday)
                        {
                            return 76;
                        }
                        else
                        {
                            return 80;
                        }
                    }
                case DayOfWeek.Saturday:
                    if (date.IsBefore("10:00pm"))
                    {
                        return 77;
                    }
                    else
                    {
                        return 76;
                    }
                case DayOfWeek.Sunday:
                    if (date.IsBefore("10:00pm"))
                    {
                        return 77;
                    }
                    else
                    {
                        return 80;
                    }
                default:
                    throw new InvalidOperationException("Unknown DayOfWeek");
            }
        }

        public override async Task UpdateAsync()
        {
            await base.UpdateAsync();

        }
    }
}
