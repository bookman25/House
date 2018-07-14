using System;
using HassSDK.Models;

namespace HouseService.ElasticSearch
{
    public class ESThermostat
    {
        public long Id => Timestamp.Ticks;

        public DateTime Timestamp { get; set; }

        public long TargetTemperature { get; set; }

        public long CurrentTemperature { get; set; }

        public OperatingState OperatingState { get; set; }

        public FanState FanState { get; set; }
    }
}
