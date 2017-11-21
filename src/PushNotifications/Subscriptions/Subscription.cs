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

        public Subscription(SubscriptionId id, SubscriberId userId, SubscriptionToken token, SubscriptionType subscriptionType)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(userId) == false) throw new ArgumentException(nameof(userId));
            if (SubscriptionToken.IsValid(token) == false) throw new ArgumentException(nameof(token));
            if (ReferenceEquals(null, subscriptionType) == true) throw new ArgumentNullException(nameof(subscriptionType));

            state = new SubscriptionState();

            IEvent evnt = new Subscribed(id, userId, token, subscriptionType);
            Apply(evnt);
        }

        public void Subscribe(SubscriberId userId, SubscriptionToken token)
        {
            if (StringTenantId.IsValid(userId) == false) throw new ArgumentException(nameof(userId));
            if (SubscriptionToken.IsValid(token) == false) throw new ArgumentException(nameof(token));

            if (state.IsSubscriptionActive == false || state.SubscriberId != userId)
            {
                IEvent evnt = new Subscribed(state.Id, userId, state.SubscriptionToken, state.SubscriptionType);
                Apply(evnt);
            }
        }

        public void UnSubscribe(SubscriberId userId, SubscriptionToken token)
        {
            if (StringTenantId.IsValid(userId) == false) throw new ArgumentException(nameof(userId));
            if (SubscriptionToken.IsValid(token) == false) throw new ArgumentException(nameof(token));

            if (state.IsSubscriptionActive == true)
            {
                IEvent evnt = new UnSubscribed(state.Id, userId, state.SubscriptionToken, state.SubscriptionType);
                Apply(evnt);
            }
        }
    }
}
