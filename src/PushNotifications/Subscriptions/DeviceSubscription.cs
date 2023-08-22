using System;
using System.Linq;
using Elders.Cronus;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class DeviceSubscription : AggregateRoot<DeviceSubscriptionState>
    {
        DeviceSubscription() { }

        public DeviceSubscription(DeviceSubscriptionId id, DeviceSubscriberId subscriberId, SubscriptionToken subscriptionToken)
        {
            if (id is null) throw new ArgumentException(nameof(id));
            if (subscriberId is null) throw new ArgumentException(nameof(subscriberId));
            if (SubscriptionToken.IsValid(subscriptionToken) == false) throw new ArgumentException(nameof(subscriptionToken));

            state = new DeviceSubscriptionState();

            IEvent evnt = new Subscribed(id, subscriberId, subscriptionToken);
            Apply(evnt);
        }

        public void Subscribe(DeviceSubscriberId subscriberId)
        {
            if (state.IsSubscriptionActive == false || state.Subscribers.Where(s=>s.Equals(subscriberId)).Any() == false)
            {
                IEvent evnt = new Subscribed(state.Id, subscriberId, state.SubscriptionToken);
                Apply(evnt);
            }
        }

        public void UnSubscribe(DeviceSubscriberId subscriberId)
        {
            if (state.IsSubscriptionActive == true)
            {
                IEvent evnt = new UnSubscribed(state.Id, subscriberId, state.SubscriptionToken);
                Apply(evnt);
            }
        }
    }
}
