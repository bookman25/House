using System;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Models;
using HouseService.Services;

namespace HouseService.Sensors
{
    public class MotionSensor : Sensor
    {
        public int Threshold { get; }

        public bool IsActive { get; private set; }

        private DateTime lastUpdate;

        public MotionSensor(HassService hass, [NotNull] string entityId, int threshold = 3)
            : base(hass, entityId)
        {
            Threshold = threshold;

            OnChanged += MotionSensor_OnChanged;
        }

        private void MotionSensor_OnChanged(object sender, EventData data)
        {
            lastUpdate = data.NewState.Date;
            IsActive = data.NewState.AsInt().State > Threshold;
        }

        public async Task<MotionSensor> RefreshAsync(HassClient client)
        {
            if (lastUpdate.AddMinutes(10) < DateTime.Now)
            {
                lastUpdate = DateTime.Now;
                var state = await client.States.GetEntityAsync(EntityId);
                IsActive = int.Parse(state.State) > Threshold;
            }
            return this;
        }
    }
}
