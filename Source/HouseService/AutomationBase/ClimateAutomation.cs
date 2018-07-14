using System;
using System.Threading.Tasks;
using HouseService.Devices;
using HouseService.ElasticSearch;
using HouseService.Sensors;
using HouseService.Services;

namespace HouseService.AutomationBase
{
    public abstract class ClimateAutomation : Automation
    {
        protected Thermostat Thermostat { get; }

        protected ElasticIndex Index { get; }

        protected ClimateAutomation(HassService hass, string entityId, GenericSensor sensor, ElasticIndex index = null)
            : base(hass)
        {
            Index = index;
            Thermostat = new Thermostat(hass, entityId, sensor);
        }

        private DateTime lastIndex;

        public override async Task UpdateAsync()
        {
            if (lastIndex.AddMinutes(1) < DateTime.UtcNow)
            {
                lastIndex = DateTime.UtcNow;
                var state = await Thermostat.GetCurrentStateAsync();
                await Index.IndexItemAsync(state);
            }
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
