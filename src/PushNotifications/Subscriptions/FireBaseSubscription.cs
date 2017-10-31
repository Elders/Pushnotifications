using System;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;
using PushNotifications.Contracts.FireBaseSubscriptions;
using PushNotifications.Contracts.FireBaseSubscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class FireBaseSubscription : AggregateRoot<FireBaseSubscriptionState>
    {
        FireBaseSubscription() { }

        public FireBaseSubscription(FireBaseSubscriptionId id, SubscriberId userId, SubscriptionToken token)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(userId) == false) throw new ArgumentException(nameof(userId));
            if (SubscriptionToken.IsValid(token) == false) throw new ArgumentException(nameof(token));

            state = new FireBaseSubscriptionState();

            IEvent evnt = new UserSubscribedForFireBase(id, userId, token);
            Apply(evnt);
        }

        public void Subscribe(SubscriberId userId, SubscriptionToken token)
        {
            if (StringTenantId.IsValid(userId) == false) throw new ArgumentException(nameof(userId));
            if (SubscriptionToken.IsValid(token) == false) throw new ArgumentException(nameof(token));

            if (state.IsSubscriptionActive == false && state.UserId != userId)
            {
                IEvent evnt = new UserSubscribedForFireBase(state.Id, userId, state.Token);
                Apply(evnt);
            }
        }

        public void UnSubscribe(SubscriberId userId, SubscriptionToken token)
        {
            if (StringTenantId.IsValid(userId) == false) throw new ArgumentException(nameof(userId));
            if (SubscriptionToken.IsValid(token) == false) throw new ArgumentException(nameof(token));

            if (state.IsSubscriptionActive == true)
            {
                IEvent evnt = new UserUnSubscribedFromFireBase(state.Id, userId, state.Token);
                Apply(evnt);
            }
        }
    }
}
