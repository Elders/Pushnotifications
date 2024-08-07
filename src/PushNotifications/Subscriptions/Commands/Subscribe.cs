﻿using System;
using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions.Commands
{
    [DataContract(Name = "61948e19-1087-4f93-90f3-f39d448cbaba")]
    public class Subscribe : ICommand
    {
        Subscribe()
        {
            Timestamp = DateTimeOffset.UtcNow;
        }

        public Subscribe(DeviceSubscriptionId id, DeviceSubscriberId subscriberId, SubscriptionToken subscriptionToken) : this()
        {
            if (id is null) throw new ArgumentException(nameof(id));
            if (subscriberId is null) throw new ArgumentException(nameof(subscriberId));
            if (SubscriptionToken.IsValid(subscriptionToken) == false) throw new ArgumentException(nameof(subscriptionToken));

            Id = id;
            SubscriberId = subscriberId;
            SubscriptionToken = subscriptionToken;
        }

        [DataMember(Order = 1)]
        public DeviceSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public DeviceSubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 3)]
        public SubscriptionToken SubscriptionToken { get; private set; }

        [DataMember(Order = 4)]
        public DateTimeOffset Timestamp { get; private set; }
    }
}
