using System;
using HassSDK;
using HouseService.Services;

namespace HouseService.Sensors
{
    public class GenericSensor : Sensor
    {
        public GenericSensor(HassService hass, [NotNull] string entityId)
            : base(hass, entityId)
        {
        }
    }
}
