using System;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class Subscription : AggregateRoot<SubscriptionState>
    {
        Subscription() { }

        public Subscription(SubscriptionId id, SubscriberId subscriberId, SubscriptionToken subscriptionToken)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (SubscriptionToken.IsValid(subscriptionToken) == false) throw new ArgumentException(nameof(subscriptionToken));

            state = new SubscriptionState();

            IEvent evnt = new Subscribed(id, subscriberId, subscriptionToken);
            Apply(evnt);
        }

        public void Subscribe(SubscriberId subscriberId)
        {
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));

            if (state.IsSubscriptionActive == false || state.SubscriberId != subscriberId)
            {
                IEvent evnt = new Subscribed(state.Id, subscriberId, state.SubscriptionToken);
                Apply(evnt);
            }
        }

        public void UnSubscribe(SubscriberId subscriberId)
        {
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));

            if (state.IsSubscriptionActive == true)
            {
                IEvent evnt = new UnSubscribed(state.Id, subscriberId, state.SubscriptionToken);
                Apply(evnt);
            }
        }
    }
}
