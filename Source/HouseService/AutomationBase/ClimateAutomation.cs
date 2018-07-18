using System;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Models;
using HouseService.Devices;
using HouseService.ElasticSearch;
using HouseService.Sensors;
using HouseService.Services;
using Microsoft.Extensions.Logging;

namespace HouseService.AutomationBase
{
    public abstract class ClimateAutomation : Automation
    {
        public abstract string Name { get; }

        protected Thermostat Thermostat { get; }

        protected ElasticIndex Index { get; }

        protected ILogger Logger { get; }

        private TimeSpan HoldDuration { get; } = TimeSpan.FromMinutes(60);

        protected ClimateAutomation(HassService hass, [NotNull] string entityId, GenericSensor sensor, ElasticIndex index, ILogger logger)
            : base(hass)
        {
            Index = index;
            Logger = logger;
            Thermostat = new Thermostat(hass, entityId);

            sensor.OnChanged += Sensor_OnChanged;
        }

        private async void Sensor_OnChanged(object sender, EventData e)
        {
            if (lastUpdate.AddSeconds(5) > DateTime.UtcNow)
            {
                return;
            }

            var currentState = await Thermostat.GetCurrentStateAsync();
            if (currentState.TargetTemperature != holdTemp && currentState.TargetTemperature != GetTimeBasedTargetTemperature())
            {
                Logger.LogInformation($"[{Name}] Starting temperature hold. " +
                    $"Holding at {currentState.TargetTemperature} until {DateTime.Now.Add(HoldDuration).ToShortTimeString()}");
                holdStartTime = DateTime.UtcNow;
                holdTemp = currentState.TargetTemperature;
            }
        }

        private DateTime lastIndex;

        private DateTime lastUpdate;

        private DateTime? holdStartTime;

        private long? holdTemp;

        public override async Task UpdateAsync()
        {
            var state = await Thermostat.GetCurrentStateAsync();
            if (lastIndex.AddMinutes(1) < DateTime.UtcNow)
            {
                lastIndex = DateTime.UtcNow;
                await Index.IndexItemAsync(state);
            }

            var newTargetTemp = GetTimeBasedTargetTemperature();
            if (holdStartTime.HasValue)
            {
                if (holdStartTime.GetValueOrDefault().Add(HoldDuration) < DateTime.UtcNow)
                {
                    holdStartTime = null;
                    holdTemp = null;
                    Logger.LogInformation($"[{Name}] Ending temperature hold.");
                }
                else
                {
                    return;
                }
            }

            if (await SetTemperatureAsync(newTargetTemp))
            {
                Logger.LogTrace("Temperature successfully set to {newTargetTemp}", newTargetTemp);
                lastUpdate = DateTime.UtcNow;
            }
        }

        protected abstract int GetTimeBasedTargetTemperature();

        protected virtual Task<long> GetCurrentTemperatureAsync()
        {
            return Thermostat.GetCurrentTemperatureAsync();
        }

        protected ValueTask<bool> SetTemperatureAsync(int temp)
        {
            if (holdStartTime != null)
            {
                return new ValueTask<bool>(false);
            }
            return Thermostat.SetTemperatureAsync(temp);
        }
    }
}
