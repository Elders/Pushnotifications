using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using System;

namespace PushNotifications.Subscriptions
{
    public class GCMSubscriptionState : AggregateRootState<GCMSubscription, GCMSubscriptionId>
    {
        public override GCMSubscriptionId Id { get; set; }

        public string UserId { get; set; }

        public string Token { get; set; }

        public void When(UserSubscribedForGCM e)
        {
            if (ReferenceEquals(null, e)) throw new ArgumentNullException("e");

            Id = e.Id;
            UserId = e.UserId;
            Token = e.Token;
        }

        public void When(UserUnSubscribedFromGCM e)
        {
            if (ReferenceEquals(null, e)) throw new ArgumentNullException("e");

            Id = e.Id;
            UserId = string.Empty;
            Token = e.Token;
        }
    }
}