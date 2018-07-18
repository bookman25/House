using System;
using Newtonsoft.Json;

namespace HassSDK.Requests
{
    public class LightChangeRequest : EntityRequest
    {
        [JsonProperty("brightness")]
        public int? Brightness { get; }

        /// <summary>
        /// Range from blue to orange (usually 153 to 500)
        /// </summary>
        [JsonProperty("color_temp")]
        public int? ColorTemp { get; }

        /// <summary>
        /// A human readable color name. eg "red"
        /// </summary>
        [JsonProperty("color_name")]
        public string ColorName { get; }

        /// <summary>
        /// Color in RGB format. eg "[255, 128, 128]"
        /// </summary>
        [JsonProperty("rgb_color")]
        public int[] RgbColor { get; }

        /// <summary>
        /// Transition duration in seconds
        /// </summary>
        [JsonProperty("transition")]
        public int? Transition { get; }

        public LightChangeRequest([NotNull] string entityId, int? transition)
            : base(entityId)
        {
            Transition = transition;
        }

        public LightChangeRequest([NotNull] string entityId, int? colorTemp, int? brightness, int? transition)
            : base(entityId)
        {
            ColorTemp = colorTemp;
            Brightness = brightness;
            Transition = transition;
        }

        public LightChangeRequest([NotNull] string entityId, string colorName, int? brightness, int? transition)
            : base(entityId)
        {
            ColorName = colorName;
            Brightness = brightness;
            Transition = transition;
        }

        public LightChangeRequest([NotNull] string entityId, int[] rgbColor, int? brightness, int? transition)
            : base(entityId)
        {
            RgbColor = rgbColor;
            if (rgbColor != null && rgbColor.Length != 3)
            {
                throw new ArgumentException("RGB array is the wrong size");
            }
            Brightness = brightness;
            Transition = transition;
        }

        public static bool operator ==(LightChangeRequest a, LightChangeRequest b)
        {
            return a?.Equals(b) ?? b == null;
        }

        public static bool operator !=(LightChangeRequest a, LightChangeRequest b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is LightChangeRequest other)
            {
                return other.Brightness == this.Brightness &&
                    other.ColorName == this.ColorName &&
                    other.ColorTemp == this.ColorTemp &&
                    other.EntityId == this.EntityId &&
                    other.Transition == this.Transition &&
                    CompareRgb(other.RgbColor);
            }

            return false;
        }

        private bool CompareRgb(int[] rgb)
        {
            if (rgb == null && this.RgbColor == null)
            {
                return true;
            }

            if (rgb != null || this.RgbColor != null)
            {
                return false;
            }

            return rgb[0] == this.RgbColor[0] && rgb[1] == this.RgbColor[1] && rgb[2] == this.RgbColor[2];
        }
    }
}
