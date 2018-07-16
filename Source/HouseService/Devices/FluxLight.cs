using System;
using System.Threading.Tasks;
using HassSDK.Requests;
using HouseService.Services;

namespace HouseService.Devices
{
    public class FluxLight : LightGroup
    {
        public FluxLight(HassService hass, string entityId)
            : base(hass, entityId)
        {
        }

        public Task ChangeLevelsAsync(int? brightness = null, int? blueLevel = null, int? transitionTime = null)
        {
            var request = new LightChangeRequest
            {
                EntityId = EntityId,
                Brightness = brightness,
                ColorTemp = BlueToColorTemp(blueLevel),
                Transition = transitionTime
            };
            return ChangeLevelsAsync(request);
        }

        private int? BlueToColorTemp(int? bluePercent)
        {
            if (bluePercent == null)
            {
                return null;
            }

            bluePercent = 100 - bluePercent;
            var blue = 153;
            var orange = 500;

            var range = orange - blue;
            var newValue = range * bluePercent / 100f;
            return blue + (int)newValue;
        }
    }
}
