using System;
using System.Threading.Tasks;
using HassSDK.Models;
using HassSDK.Requests;
using HouseService.Sensors;
using HouseService.Services;

namespace HouseService.Devices
{
    public class Thermostat : Device
    {
        private readonly GenericSensor sensor;

        public Thermostat(HassService hass, string entityId, GenericSensor sensor)
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

            var domain = await GetDomainAsync();
            if (domain == null)
            {
                return false;
            }

            var setTemp = domain.Services["set_temperature"];
            await Client.Services.CallServiceAsync(setTemp, new ThermostatChangeRequest { EntityId = EntityId, TargetTemperature = temp });
            return true;
        }
    }
}
