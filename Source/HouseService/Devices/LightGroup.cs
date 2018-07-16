using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
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

        protected LightGroup(HassService hass, string entityId)
            : base(hass, "light", entityId)
        {
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
            var domain = await GetDomainAsync();
            if (domain == null)
            {
                return;
            }

            IsOn = false;
            var turnOn = domain.Services["turn_off"];
            await Client.Services.CallServiceAsync(turnOn, new LightChangeRequest(EntityId, transition));
        }

        protected async Task ChangeLevelsAsync(LightChangeRequest state)
        {
            currentLevels = state;
            await RefreshSensorsAsync();
            var currentState = await Client.States.GetEntityAsync(EntityId);
            if (currentState.State != "off")
            {
                await TurnOnAsync(state);
            }
        }

        private async Task TurnOnAsync(LightChangeRequest state)
        {
            var domain = await GetDomainAsync();
            if (domain == null)
            {
                return;
            }

            IsOn = true;
            var turnOn = domain.Services["turn_on"];
            if (state.EntityId != EntityId)
            {
                throw new InvalidOperationException("Tried to change state of another light.");
            }
            await Client.Services.CallServiceAsync(turnOn, state);
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

        private async Task RefreshSensorsAsync()
        {
            if (MotionSensors.Count == 0)
            {
                return;
            }

            await Task.WhenAll(MotionSensors.Values.Select(i => i.RefreshAsync(Client)));

            await HandleSensorChangeAsync();
        }
    }
}
