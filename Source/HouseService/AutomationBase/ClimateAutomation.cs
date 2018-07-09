using System;
using System.Threading.Tasks;
using HouseService.DeviceTypes;
using HouseService.Sensors;
using HouseService.Services;

namespace HouseService.AutomationBase
{
    public abstract class ClimateAutomation : Automation
    {
        private readonly Thermostat thermostat;

        protected ClimateAutomation(HassService hass, string entityId, GenericSensor sensor)
            : base(hass)
        {
            thermostat = new Thermostat(hass, entityId, sensor);
        }

        protected virtual Task<long> GetCurrentTemperatureAsync()
        {
            return thermostat.GetCurrentTemperatureAsync();
        }

        protected Task SetTemperatureAsync(int temp)
        {
            return thermostat.SetTemperatureAsync(temp);
        }
    }
}
