using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using System;

namespace PushNotifications.Subscriptions
{
    public class ParseSubscriptionState : AggregateRootState<ParseSubscription, ParseSubscriptionId>
    {
        public override ParseSubscriptionId Id { get; set; }

        public string UserId { get; set; }

        public string Token { get; set; }

        public void When(UserSubscribedForParse e)
        {
            if (ReferenceEquals(null, e)) throw new ArgumentNullException("e");

            Id = e.Id;
            UserId = e.UserId;
            Token = e.Token;
        }

        public void When(UserUnSubscribedFromParse e)
        {
            if (ReferenceEquals(null, e)) throw new ArgumentNullException("e");

            Id = e.Id;
            UserId = string.Empty;
            Token = e.Token;
        }
    }
}