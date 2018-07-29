using System;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Models;
using HouseService.Api.Models;
using HouseService.Devices;
using HouseService.ElasticSearch;
using HouseService.Sensors;
using HouseService.Services;
using Microsoft.Extensions.Logging;

namespace HouseService.AutomationBase
{
    public abstract class ClimateAutomation : Automation
    {
        public override AutomationType Type => AutomationType.Climate;

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
            if (lastUpdate.AddSeconds(5) > DateTime.Now)
            {
                return;
            }

            var currentState = await Thermostat.GetCurrentStateAsync();
            CurrentTemp = currentState.CurrentTemperature;
            if (currentState.TargetTemperature != HoldTemp && currentState.TargetTemperature != GetTimeBasedTargetTemperature())
            {
                HoldStartTime = DateTime.Now;
                HoldTemp = currentState.TargetTemperature;
                Logger.LogInformation($"[{Name}] Starting temperature hold. Holding at {HoldTemp} until {HoldEndTime.GetValueOrDefault().ToShortTimeString()}");
            }
        }

        private DateTime lastIndex;

        private DateTime lastUpdate;

        public DateTime? HoldStartTime { get; private set; }

        public DateTime? HoldEndTime => HoldStartTime.GetValueOrDefault().Add(HoldDuration);

        public long? HoldTemp { get; private set; }

        public long? CurrentTemp { get; private set; }

        public override Task LoadAsync()
        {
            return Task.CompletedTask;
        }

        public override async Task UpdateAsync()
        {
            var state = await Thermostat.GetCurrentStateAsync();
            CurrentTemp = state.CurrentTemperature;
            if (lastIndex.AddMinutes(1) < DateTime.Now)
            {
                lastIndex = DateTime.Now;
                await Index.IndexItemAsync(state);
            }

            var newTargetTemp = GetTimeBasedTargetTemperature();
            if (HoldStartTime.HasValue)
            {
                if (HoldStartTime.GetValueOrDefault().Add(HoldDuration) < DateTime.Now)
                {
                    HoldStartTime = null;
                    HoldTemp = null;
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
                lastUpdate = DateTime.Now;
            }
        }

        protected abstract int GetTimeBasedTargetTemperature();

        protected virtual Task<long> GetCurrentTemperatureAsync()
        {
            return Thermostat.GetCurrentTemperatureAsync();
        }

        protected ValueTask<bool> SetTemperatureAsync(int temp)
        {
            if (HoldStartTime != null)
            {
                return new ValueTask<bool>(false);
            }
            return Thermostat.SetTemperatureAsync(temp);
        }

        public override AutomationViewModel CreateViewModel()
        {
            return new ClimateAutomationViewModel(this);
        }
    }
}
