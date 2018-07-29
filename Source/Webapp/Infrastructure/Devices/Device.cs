using System;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Requests;
using HouseService.Services;
using Microsoft.Extensions.Logging;

namespace HouseService.Devices
{
    public abstract class Device
    {
        protected HassService HassService { get; }

        protected HassClient Client => HassService.Client;

        protected string DomainKey { get; }

        [NotNull]
        public string EntityId { get; }

        protected Device(HassService hass, string domain, [NotNull] string entityId)
        {
            HassService = hass;
            DomainKey = domain;
            EntityId = entityId;
        }

        public async Task<bool> ExecuteServiceAsync(string serviceName, EntityRequest request)
        {
            var domain = await HassService.GetDomainAsync(DomainKey);
            if (domain == null)
            {
                return false;
            }

            LogHelper.DefaultLogger.LogTrace($"Calling {request.EntityId}.{serviceName}");
            return await domain.GetService(serviceName).ExecuteAsync(request);
        }
    }
}
