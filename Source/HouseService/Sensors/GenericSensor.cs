using System;
using HassSDK;

namespace HouseService.Sensors
{
    public class GenericSensor : Sensor
    {
        public GenericSensor(SubscriptionClient subscriptionClient, [NotNull] string entityId)
            : base(subscriptionClient, entityId)
        {
        }
    }
}
