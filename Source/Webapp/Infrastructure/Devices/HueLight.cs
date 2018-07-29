using System;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Requests;
using HouseService.Services;

namespace HouseService.Devices
{
    public class HueLight : LightGroup
    {
        public HueLight(HassService hass, [NotNull] string entityId)
            : base(hass, entityId)
        {
        }

        public Task ChangeColorAsync(string color, int? brightness = null, int? transitionTime = null)
        {
            var request = new LightChangeRequest(EntityId, color, brightness, transitionTime);
            return ChangeLevelsAsync(request);
        }

        public Task ChangeColorAsync(int red, int green, int blue, int? brightness = null, int? transitionTime = null)
        {
            var request = new LightChangeRequest(EntityId, new int[] { red, green, blue }, brightness, transitionTime);
            return ChangeLevelsAsync(request);
        }
    }
}
