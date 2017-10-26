using Elders.Cronus.DomainModeling;
using System;
using PushNotifications.Contracts;
using PushNotifications.Contracts.FireBaseSubscriptions;
using PushNotifications.Contracts.FireBaseSubscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class FireBaseSubscriptionState : AggregateRootState<FireBaseSubscription, FireBaseSubscriptionId>
    {
        public override FireBaseSubscriptionId Id { get; set; }

        public UserId UserId { get; set; }

        public SubscriptionToken Token { get; set; }

        public bool IsSubscriptionActive { get; set; }

        public void When(UserSubscribedForFireBase e)
        {
            if (ReferenceEquals(null, e) == true) throw new ArgumentNullException(nameof(e));

            Id = e.Id;
            UserId = e.UserId;
            Token = e.Token;
            IsSubscriptionActive = true;
        }

        public void When(UserUnSubscribedFromFireBase e)
        {
            if (ReferenceEquals(null, e) == true) throw new ArgumentNullException(nameof(e));
            IsSubscriptionActive = false;
        }
    }
}
