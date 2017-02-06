using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using System;

namespace PushNotifications.Subscriptions
{
    public class PushySubscriptionState : AggregateRootState<PushySubscription, PushySubscriptionId>
    {
        public override PushySubscriptionId Id { get; set; }

        public string UserId { get; set; }

        public string Token { get; set; }

        public void When(UserSubscribedForPushy e)
        {
            if (ReferenceEquals(null, e)) throw new ArgumentNullException(nameof(e));

            Id = e.Id;
            UserId = e.UserId;
            Token = e.Token;
        }

        public void When(UserUnSubscribedFromPushy e)
        {
            if (ReferenceEquals(null, e)) throw new ArgumentNullException(nameof(e));

            Id = e.Id;
            UserId = string.Empty;
            Token = e.Token;
        }
    }
}
