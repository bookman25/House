using System;
using System.Diagnostics;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Models;
using HassSDK.Requests;
using HouseService.Sensors;
using HouseService.Services;

namespace HouseService.Devices
{
    public class Computer : Device
    {
        public Computer(HassService hass, [NotNull] string entityId)
            : base(hass, "switch", entityId)
        {
            hass.Subscribe(entityId, OnChange);
        }

        private void OnChange(EventData data)
        {
            if (data.NewState.State == "off")
            {
                new Process() { StartInfo = new ProcessStartInfo(@"C:\Program Files\nircmd\nircmd.exe", "cmdwait 1000 monitor off") }.Start();
            }
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
