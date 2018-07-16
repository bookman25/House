using System;
using System.Threading.Tasks;
using HassSDK.Requests;
using HouseService.Services;

namespace HouseService.Devices
{
    public class HueLight : LightGroup
    {
        public HueLight(HassService hass, string entityId)
            : base(hass, entityId)
        {
        }

        public Task ChangeColorAsync(string color, int? brightness = null, int? transitionTime = null)
        {
            var request = new LightChangeRequest
            {
                EntityId = EntityId,
                Brightness = brightness,
                ColorName = color,
                Transition = transitionTime
            };
            return ChangeLevelsAsync(request);
        }

        public Task ChangeColorAsync(int red, int green, int blue, int? brightness = null, int? transitionTime = null)
        {
            var request = new LightChangeRequest
            {
                EntityId = EntityId,
                Brightness = brightness,
                RgbColor = new int[] { red, green, blue },
                Transition = transitionTime
            };
            return ChangeLevelsAsync(request);
        }
    }
}
