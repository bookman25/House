using System;
using Newtonsoft.Json;

namespace HassSDK.Requests
{
    public class LightChangeRequest : EntityRequest
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

        public LightChangeRequest WithTransition(int? transition)
        {
            return new LightChangeRequest { Brightness = Brightness, ColorTemp = ColorTemp, WhiteValue = WhiteValue, Transition = transition };
        }
    }
}
