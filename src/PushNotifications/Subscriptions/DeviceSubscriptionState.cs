using Elders.Cronus;
using PushNotifications.Subscriptions.Events;
using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.Subscriptions
{
    public class DeviceSubscriptionState : AggregateRootState<DeviceSubscription, DeviceSubscriptionId>
    {
        public DeviceSubscriptionState()
        {
            Subscribers = new HashSet<DeviceSubscriberId>();
        }

        public override DeviceSubscriptionId Id { get; set; }

        public HashSet<DeviceSubscriberId> Subscribers { get; set; }

        public SubscriptionToken SubscriptionToken { get; set; }

        public SubscriptionType SubscriptionType { get; set; }

        public bool IsSubscriptionActive { get; set; }

        public void When(Subscribed e)
        {
            Id = e.Id;
            Subscribers.Add( e.SubscriberId);
            SubscriptionToken = e.SubscriptionToken;
            IsSubscriptionActive = true;
        }

        public void When(UnSubscribed e)
        {
            Subscribers.Remove(e.SubscriberId);
            
            if(Subscribers.Any() == false)
                IsSubscriptionActive = false;
        }
    }
}
