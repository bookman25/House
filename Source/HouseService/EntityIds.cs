using System;
using HassSDK;

namespace HouseService
{
    public static class EntityIds
    {
        [NotNull]
        public const string DownstairsThermostatCooling = "climate.linear_unknown_type5442_id5437_cooling_1_2";

        [NotNull]
        public const string DownstairsThermostat = "zwave.linear_unknown_type5442_id5437_2";

        [NotNull]
        public const string UpstairsThermostatCooling = "climate.linear_unknown_type5442_id5437_cooling_1";

        [NotNull]
        public const string UpstairsThermostat = "zwave.linear_unknown_type5442_id5437";

        [NotNull]
        public const string KitchenMotionSensor = "sensor.aeotec_zw100_multisensor_6_burglar";

        [NotNull]
        public const string KitchenTemperatureSensor = "sensor.aeotec_zw100_multisensor_6_temperature";

        [NotNull]
        public const string KitchenLight = "light.kitchen";

        [NotNull]
        public const string PantryLight = "light.pantry";

        [NotNull]
        public const string LivingRoomLight = "light.living_room";
    }
}
