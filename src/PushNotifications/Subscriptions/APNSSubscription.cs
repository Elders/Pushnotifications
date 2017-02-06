using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using System;

namespace PushNotifications.Subscriptions
{
    public class APNSSubscription : AggregateRoot<APNSSubscriptionState>
    {
        APNSSubscription() { }

        public APNSSubscription(APNSSubscriptionId id, string userId, string token)
        {
            if (ReferenceEquals(null, id)) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

            state = new APNSSubscriptionState();
            IEvent evnt = new UserSubscribedForAPNS(id, userId, token);
            Apply(evnt);
        }

        public void Subscribe(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

            if (state.UserId != userId && state.Token == token)
            {
                IEvent evnt = new UserSubscribedForAPNS(state.Id, userId, state.Token);
                Apply(evnt);
            }
        }

        public void UnSubscribe(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

            if (state.UserId == userId && state.Token == token)
            {
                IEvent evnt = new UserUnSubscribedFromAPNS(state.Id, userId, state.Token);
                Apply(evnt);
            }
        }
    }
}
