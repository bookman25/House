using System;
using Newtonsoft.Json;

namespace HassSDK.Requests
{
    public class LightChangeRequest : EntityRequest
    {
        [JsonProperty("brightness")]
        public int? Brightness { get; set; }

        /// <summary>
        /// Range from blue to orange (usually 153 to 500)
        /// </summary>
        [JsonProperty("color_temp")]
        public int? ColorTemp { get; set; }

        /// <summary>
        /// A human readable color name. eg "red"
        /// </summary>
        [JsonProperty("color_name")]
        public string ColorName { get; set; }

        /// <summary>
        /// Color in RGB format. eg "[255, 128, 128]"
        /// </summary>
        [JsonProperty("rgb_color")]
        public int[] RgbColor { get; set; }

        /// <summary>
        /// Transition duration in seconds
        /// </summary>
        [JsonProperty("transition")]
        public int? Transition { get; set; }

        public LightChangeRequest WithTransition(int? transition)
        {
            return new LightChangeRequest
            {
                Brightness = Brightness,
                ColorTemp = ColorTemp,
                ColorName = ColorName,
                RgbColor = RgbColor,
                Transition = transition
            };
        }

        public LightChangeRequest()
        {
        }

        public LightChangeRequest(string entityId, int? transition)
        {
            EntityId = entityId;
            Transition = transition;
        }
    }
}
