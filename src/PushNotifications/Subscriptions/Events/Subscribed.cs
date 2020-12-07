using Elders.Cronus;
using System.Runtime.Serialization;
using System;

namespace PushNotifications.Subscriptions.Events
{
    [DataContract(Name = "1d1bacb3-0293-4a03-83ad-a58075084c00")]
    public class Subscribed : IEvent
    {
        Subscribed() { }

        public Subscribed(DeviceSubscriptionId id, DeviceSubscriberId subscriberId, SubscriptionToken subscriptionToken)
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
    }
}
