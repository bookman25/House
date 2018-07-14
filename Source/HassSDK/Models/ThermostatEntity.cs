using System;
using Newtonsoft.Json.Linq;

namespace HassSDK.Models
{
    public class ThermostatEntity : Entity
    {
        public long TargetTemperature { get; set; }

        public long CurrentTemperature { get; set; }

        public OperatingState OperatingState { get; set; }

        public FanState FanState { get; set; }

        protected override void SetProperties(JToken json)
        {
            base.SetProperties(json);
            TargetTemperature = GetAttribute<long>("temperature");
            CurrentTemperature = GetAttribute<long>("current_temperature");
            if (Enum.TryParse<OperatingState>(GetAttribute<string>("operating_state").Replace(" ", ""), out var operatingState))
            {
                OperatingState = operatingState;
            }
            else
            {
                throw new NotImplementedException($"Could not parse thermostate operating state: {Attributes["operating_state"]}");
            }

            if (Enum.TryParse<FanState>(GetAttribute<string>("fan_state"), out var fanState))
            {
                FanState = fanState;
            }
            else
            {
                throw new NotImplementedException($"Could not parse thermostate fan state: {Attributes["fan_state"]}");
            }
        }
    }

    public enum OperatingState
    {
        Idle,
        PendingCool,
        Cooling
    }

    public enum FanState
    {
        Running,
        Idle
    }
}
