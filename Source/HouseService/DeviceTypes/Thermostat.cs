using System;
using System.Threading.Tasks;
using HassSDK.Models;
using HouseService.Services;

namespace HouseService.DeviceTypes
{
    public class Thermostat : Device
    {
        public Thermostat(HassService hass, string entityId)
            : base(hass, "climate", entityId)
        {
        }

        public async Task<long> GetCurrentTargetTemperatureAsync()
        {
            var entity = await Client.States.GetEntityAsync(EntityId);
            return entity.GetAttribute<long>("temperature");
        }

        public async Task<long> GetCurrentTemperatureAsync()
        {
            var entity = await Client.States.GetEntityAsync(EntityId);
            return entity.GetAttribute<long>("current_temperature");
        }

        public async Task SetTemperatureAsync(int temp)
        {
            var currentTargetTemp = await GetCurrentTargetTemperatureAsync();
            if (currentTargetTemp == temp)
            {
                return;
            }

            var domain = await GetDomainAsync();
            if (domain == null)
            {
                return;
            }

            var setTemp = domain.Services["set_temperature"];
            await Client.Services.CallServiceAsync(setTemp, new ThermostatState { EntityId = EntityId, Temperature = temp });
        }
    }
}
