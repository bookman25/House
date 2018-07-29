using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Models;
using HassSDK.Requests;
using HouseService.Extensions;
using HouseService.Sensors;
using HouseService.Services;

namespace HouseService.Devices
{
    public abstract class LightGroup : Device
    {
        public ImmutableDictionary<string, MotionSensor> MotionSensors { get; private set; } = ImmutableDictionary<string, MotionSensor>.Empty;

        private LightChangeRequest currentLevels;

        public bool IsOn { get; private set; }

        protected LightGroup(HassService hass, [NotNull] string entityId)
            : base(hass, "light", entityId)
        {
        }

        public async Task LoadAsync()
        {
            var currentState = await Client.States.GetEntityAsync(EntityId);
            IsOn = currentState.State == "on";
        }

        public void AddMotionSensor(MotionSensor sensor)
        {
            sensor.OnChanged += Sensor_OnChanged;
            MotionSensors = MotionSensors.SetItem(sensor.EntityId, sensor);
        }

        public Task TurnOnAsync(int? transition = null)
        {
            return TurnOnAsync(new LightChangeRequest(EntityId, transition));
        }

        public async Task TurnOffAsync(int? transition = null)
        {
            IsOn = false;
            await ExecuteServiceAsync("turn_off", new LightChangeRequest(EntityId, transition));
        }

        public async Task ToggleAsync(int? transition = null)
        {
            IsOn = !IsOn;
            await ExecuteServiceAsync("toggle", new LightChangeRequest(EntityId, transition));
        }

        protected async Task ChangeLevelsAsync(LightChangeRequest state)
        {
            currentLevels = state;
            var currentState = await Client.States.GetEntityAsync(EntityId);
            if (currentState.State != "off")
            {
                await TurnOnAsync(state);
            }

            await RefreshSensorsAsync();
        }

        private async Task TurnOnAsync(LightChangeRequest state)
        {
            if (state.EntityId != EntityId)
            {
                throw new InvalidOperationException("Tried to change state of another light.");
            }

            IsOn = true;
            await ExecuteServiceAsync("turn_on", state);
        }

        private void Sensor_OnChanged(object sender, EventData e)
        {
            HandleSensorChangeAsync().Forget();
        }

        private async Task HandleSensorChangeAsync()
        {
            if (MotionSensors.Values.Any(i => i.IsActive))
            {
                await TurnOnAsync(currentLevels);
            }
            else
            {
                await TurnOffAsync();
            }
        }

        private DateTime lastSensorRefresh;

        public async Task RefreshSensorsAsync()
        {
            if (MotionSensors.Count == 0 || lastSensorRefresh.AddMinutes(10) > DateTime.Now)
            {
                return;
            }

            lastSensorRefresh = DateTime.Now;
            await Task.WhenAll(MotionSensors.Values.Select(i => i.RefreshAsync(Client)));

            await HandleSensorChangeAsync();
        }
    }
}
