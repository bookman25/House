using System;
using System.Threading.Tasks;
using HassSDK;

namespace HouseService.Automations
{
    public class MasterBedroomSensor
    {
        private static string NodeId = "4";

        public double Temperature { get; private set; }

        public MasterBedroomSensor()
        {
        }

        public async Task<MasterBedroomSensor> RefreshAsync(HassClient client)
        {
            var state = await client.States.GetEntityAsync("sensor.aeotec_zw100_multisensor_6_temperature");
            Temperature = double.Parse(state.State);
            //var states = await client.States.GetAsync();
            //var sensors = states.Values.Where(i => i.NodeId == 4).ToDictionary(i => i.EntityId);

            //Temperature = int.Parse(sensors["sensor.aeotec_zw100_multisensor_6_temperature"].State);
            return this;
        }
    }
}
