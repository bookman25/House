using System;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Models;
using HassSDK.Requests;
using HouseService.Sensors;
using HouseService.Services;

namespace HouseService.Devices
{
    public class Thermostat : Device
    {
        public Thermostat(HassService hass, [NotNull] string entityId)
            : base(hass, "climate", entityId)
        {
        }

        public Task<ThermostatEntity> GetCurrentStateAsync()
        {
            return Client.States.GetEntityAsync<ThermostatEntity>(EntityId);
        }

        public async Task<long> GetCurrentTargetTemperatureAsync()
        {
            var entity = await Client.States.GetEntityAsync<ThermostatEntity>(EntityId);
            return entity.TargetTemperature;
        }

        public async Task<long> GetCurrentTemperatureAsync()
        {
            var entity = await Client.States.GetEntityAsync<ThermostatEntity>(EntityId);
            return entity.CurrentTemperature;
        }

        public async ValueTask<bool> SetTemperatureAsync(int temp)
        {
            var currentTargetTemp = await GetCurrentTargetTemperatureAsync();
            if (currentTargetTemp == temp)
            {
                return false;
            }

            return await ExecuteServiceAsync("set_temperature", new ThermostatChangeRequest(EntityId, temp));
        }
    }
}
