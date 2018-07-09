using System;
using System.Threading.Tasks;
using HassSDK;

namespace HouseService.Sensors
{
    public class KitchenSensor
    {
        private static string NodeId = "4";

        public double Temperature { get; private set; }

        public KitchenSensor()
        {
        }

        public async Task<KitchenSensor> RefreshAsync(HassClient client)
        {
            var state = await client.States.GetEntityAsync(EntityIds.KitchenTemperatureSensor);
            Temperature = double.Parse(state.State);
            //var states = await client.States.GetAsync();
            //var sensors = states.Values.Where(i => i.NodeId == 4).ToDictionary(i => i.EntityId);

            //Temperature = int.Parse(sensors["sensor.aeotec_zw100_multisensor_6_temperature"].State);
            return this;
        }
    }
}
