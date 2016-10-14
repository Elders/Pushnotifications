using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using System;

namespace PushNotifications.Subscriptions
{
    public class ParseSubscription : AggregateRoot<ParseSubscriptionState>
    {
        ParseSubscription() { }

        public ParseSubscription(ParseSubscriptionId id, string userId, string token)
        {
            if (ReferenceEquals(null, id)) throw new ArgumentNullException("id");
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException("userId");
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");

            state = new ParseSubscriptionState();
            IEvent evnt = new UserSubscribedForParse(id, userId, token);
            Apply(evnt);
        }

        public void Subscribe(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException("userId");
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");

            if (state.UserId != userId && state.Token == token)
            {
                IEvent evnt = new UserSubscribedForParse(state.Id, userId, state.Token);
                Apply(evnt);
            }
        }

        public void UnSubscribe(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException("userId");
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");

            if (state.UserId == userId && state.Token == token)
            {
                IEvent evnt = new UserUnSubscribedFromParse(state.Id, userId, state.Token);
                Apply(evnt);
            }
        }
    }
}