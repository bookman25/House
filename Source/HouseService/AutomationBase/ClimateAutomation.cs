using System;
using System.Threading.Tasks;
using HouseService.Devices;
using HouseService.Sensors;
using HouseService.Services;

namespace HouseService.AutomationBase
{
    public abstract class ClimateAutomation : Automation
    {
        protected Thermostat Thermostat { get; }

        protected ClimateAutomation(HassService hass, string entityId, GenericSensor sensor)
            : base(hass)
        {
            Thermostat = new Thermostat(hass, entityId, sensor);
        }

        protected virtual Task<long> GetCurrentTemperatureAsync()
        {
            return Thermostat.GetCurrentTemperatureAsync();
        }

        protected Task SetTemperatureAsync(int temp)
        {
            return Thermostat.SetTemperatureAsync(temp);
        }
    }
}
