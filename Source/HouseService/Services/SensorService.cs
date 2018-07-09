using System;
using HassSDK;
using HouseService.Sensors;

namespace HouseService.Services
{
    public class SensorService
    {
        private SubscriptionClient SubscriptionClient { get; }

        public SensorService(SubscriptionClient subscriptionClient)
        {
            SubscriptionClient = subscriptionClient;

            KitchenMotionSensor = new MotionSensor(SubscriptionClient, EntityIds.KitchenMotionSensor);
            DownstairsThermostat = new GenericSensor(SubscriptionClient, EntityIds.DownstairsThermostat);
            UpstairsThermostat = new GenericSensor(SubscriptionClient, EntityIds.UpstairsThermostat);
        }

        public MotionSensor KitchenMotionSensor { get; }

        public GenericSensor DownstairsThermostat { get; }

        public GenericSensor UpstairsThermostat { get; }
    }
}
