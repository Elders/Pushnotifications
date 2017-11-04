using System;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushySubscriptions;
using PushNotifications.Contracts.PushySubscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class PushySubscription : AggregateRoot<PushySubscriptionState>
    {
        PushySubscription() { }

        public PushySubscription(PushySubscriptionId id, SubscriberId userId, SubscriptionToken token)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(userId) == false) throw new ArgumentException(nameof(userId));
            if (SubscriptionToken.IsValid(token) == false) throw new ArgumentException(nameof(token));

            state = new PushySubscriptionState();

            IEvent evnt = new SubscriberSubscribedForPushy(id, userId, token);
            Apply(evnt);
        }

        public void Subscribe(SubscriberId userId, SubscriptionToken token)
        {
            if (StringTenantId.IsValid(userId) == false) throw new ArgumentException(nameof(userId));
            if (SubscriptionToken.IsValid(token) == false) throw new ArgumentException(nameof(token));

            if (state.IsSubscriptionActive == false || state.UserId != userId)
            {
                IEvent evnt = new SubscriberSubscribedForPushy(state.Id, userId, state.Token);
                Apply(evnt);
            }
        }

        public void UnSubscribe(SubscriberId userId, SubscriptionToken token)
        {
            if (StringTenantId.IsValid(userId) == false) throw new ArgumentException(nameof(userId));
            if (SubscriptionToken.IsValid(token) == false) throw new ArgumentException(nameof(token));

            if (state.IsSubscriptionActive == true)
            {
                IEvent evnt = new SubscriberUnSubscribedFromPushy(state.Id, userId, state.Token);
                Apply(evnt);
            }
        }
    }
}
