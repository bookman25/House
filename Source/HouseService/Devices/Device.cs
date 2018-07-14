using System;
using System.Threading.Tasks;
using HassSDK;
using HassSDK.Models;
using HouseService.Services;

namespace HouseService.Devices
{
    public abstract class Device
    {
        protected HassService HassService { get; }

        protected HassClient Client => HassService.Client;

        protected string DomainKey { get; }

        protected string EntityId { get; }

        protected Device(HassService hass, string domain, string entityId)
        {
            HassService = hass;
            DomainKey = domain;
            EntityId = entityId;
        }

        public Task<Domain> GetDomainAsync()
        {
            return HassService.GetDomainAsync(DomainKey);
        }
    }
}
