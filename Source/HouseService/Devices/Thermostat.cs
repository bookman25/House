using System;
using System.Threading.Tasks;
using HassSDK.Models;
using HassSDK.Requests;
using HouseService.Sensors;
using HouseService.Services;
using Serilog;

namespace HouseService.Devices
{
    public class Thermostat : Device
    {
        private readonly GenericSensor sensor;

        public Thermostat(HassService hass, string entityId, GenericSensor sensor)
            : base(hass, "climate", entityId)
        {
            if (sensor != null)
            {
                sensor.OnChanged += Sensor_OnChanged;
            }
        }

        private async void Sensor_OnChanged(object sender, EventData e)
        {
            // TODO: HOLD
            var target = await GetCurrentTargetTemperatureAsync();
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
            await Client.Services.CallServiceAsync(setTemp, new ThermostatChangeRequest { EntityId = EntityId, TargetTemperature = temp });
        }
    }
}
