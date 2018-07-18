using System;
using HassSDK;
using HassSDK.Models;
using HouseService.Services;

namespace HouseService.Sensors
{
    public abstract class Sensor
    {
        [NotNull]
        public string EntityId { get; }

        public event EventHandler<EventData> OnChanged;

        public Sensor(HassService hass, [NotNull] string entityId)
        {
            EntityId = entityId;
            hass.Subscribe(EntityId, EventReceived);
        }

        private void EventReceived(EventData data)
        {
            OnChanged?.Invoke(this, data);
        }
    }
}
