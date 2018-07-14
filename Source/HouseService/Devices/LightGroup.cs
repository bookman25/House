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
    public class LightGroup : Device
    {
        public ImmutableDictionary<string, MotionSensor> MotionSensors { get; private set; } = ImmutableDictionary<string, MotionSensor>.Empty;

        private LightChangeRequest currentLevels;

        public bool IsOn { get; private set; }

        public LightGroup(HassService hass, string entityId)
            : base(hass, "light", entityId)
        {
        }

        public void AddMotionSensor(MotionSensor sensor)
        {
            sensor.OnChanged += Sensor_OnChanged;
            MotionSensors = MotionSensors.SetItem(sensor.EntityId, sensor);
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
            await Task.WhenAll(MotionSensors.Values.Select(i => i.RefreshAsync(Client)));

            await HandleSensorChangeAsync();
        }

        public Task TurnOnAsync(int? brightness = null)
        {
            return TurnOnAsync(new LightChangeRequest { EntityId = EntityId, Brightness = brightness });
        }

        public async Task TurnOnAsync(LightChangeRequest state)
        {
            var domain = await GetDomainAsync();
            if (domain == null)
            {
                return;
            }

            IsOn = true;
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

            IsOn = false;
            var turnOn = domain.Services["turn_off"];
            await Client.Services.CallServiceAsync(turnOn, new LightChangeRequest { EntityId = EntityId, Transition = transition });
        }

        public async Task ChangeLevelsAsync(LightChangeRequest state)
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
