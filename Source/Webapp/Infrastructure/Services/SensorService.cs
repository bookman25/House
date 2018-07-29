using System;
using HassSDK;
using HouseService.Sensors;

namespace HouseService.Services
{
    public class SensorService
    {
        public SensorService(HassService hass)
        {
            KitchenMotionSensor = new MotionSensor(hass, EntityIds.KitchenMotionSensor);
            DownstairsThermostat = new GenericSensor(hass, EntityIds.DownstairsThermostat);
            UpstairsThermostat = new GenericSensor(hass, EntityIds.UpstairsThermostat);
        }

        public MotionSensor KitchenMotionSensor { get; }

        public GenericSensor DownstairsThermostat { get; }

        public GenericSensor UpstairsThermostat { get; }
    }
}
