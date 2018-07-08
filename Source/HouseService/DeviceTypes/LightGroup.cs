using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using HassSDK.Models;
using HouseService.Sensors;
using HouseService.Services;

namespace HouseService.DeviceTypes
{
    public class LightGroup : Device
    {
        public ImmutableDictionary<string, MotionSensor> MotionSensors { get; private set; } = ImmutableDictionary<string, MotionSensor>.Empty;

        private LightState currentLevels;

        public LightGroup(HassService hass, string entityId)
            : base(hass, "light", entityId)
        {
        }

        public void AddMotionSensor(MotionSensor sensor)
        {
            sensor.OnChanged += Sensor_OnChanged;
            MotionSensors = MotionSensors.SetItem(sensor.EntityId, sensor);
        }

        private async void Sensor_OnChanged(object sender, EventData e)
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
            await Task.WhenAll(MotionSensors.Values.Select(i => i.RefreshAsync(Client)));

            Sensor_OnChanged(null, null);
        }

        public Task TurnOnAsync(int? brightness = null)
        {
            return TurnOnAsync(new LightState { EntityId = EntityId, Brightness = brightness });
        }

        public async Task TurnOnAsync(LightState state)
        {
            var domain = await GetDomainAsync();
            if (domain == null)
            {
                return;
            }

            state.EntityId = EntityId;
            var turnOn = domain.Services["turn_on"];
            await Client.Services.CallServiceAsync(turnOn, state);
        }

        public async Task TurnOffAsync(int? transition = null)
        {
            var domain = await GetDomainAsync();
            if (domain == null)
            {
                return;
            }
            
            var turnOn = domain.Services["turn_off"];
            await Client.Services.CallServiceAsync(turnOn, new LightState { EntityId = EntityId, Transition = transition });
        }

        public async Task ChangeLevelsAsync(LightState state)
        {
            currentLevels = state;
            await RefreshSensorsAsync();
            var currentState = await Client.States.GetEntityAsync(EntityId);
            if (currentState.State != "off")
            {
                await TurnOnAsync(state);
            }
        }
    }
}
