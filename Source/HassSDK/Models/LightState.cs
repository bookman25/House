using System;
using Newtonsoft.Json;

namespace HassSDK.Models
{
    public class LightState : EntityRequest
    {
        [JsonProperty("brightness")]
        public int? Brightness { get; set; }

        [JsonProperty("color_temp")]
        public int? ColorTemp { get; set; }

        [JsonProperty("white_value")]
        public int? WhiteValue { get; set; }

        /// <summary>
        /// Transisiton duration in seconds
        /// </summary>
        [JsonProperty("transition")]
        public int? Transition { get; set; }

        public LightState WithTransition(int? transition)
        {
            return new LightState { Brightness = Brightness, ColorTemp = ColorTemp, WhiteValue = WhiteValue, Transition = transition };
        }
    }
}
