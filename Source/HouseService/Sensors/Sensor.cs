﻿using System;
using HassSDK;
using HassSDK.Models;

namespace HouseService.Sensors
{
    public abstract class Sensor
    {
        [NotNull]
        public string EntityId { get; }

        public event EventHandler<EventData> OnChanged;

        public Sensor(SubscriptionClient subscriptionClient, [NotNull] string entityId)
        {
            EntityId = entityId;
            subscriptionClient.Subscribe(EntityId, EventReceived);
        }

        private void EventReceived(EventData data)
        {
            OnChanged?.Invoke(this, data);
        }
    }
}
