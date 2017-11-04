using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;
using PushNotifications.Contracts.FireBaseSubscriptions;
using PushNotifications.Contracts.FireBaseSubscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class FireBaseSubscriptionState : AggregateRootState<FireBaseSubscription, FireBaseSubscriptionId>
    {
        public override FireBaseSubscriptionId Id { get; set; }

        public SubscriberId UserId { get; set; }

        public SubscriptionToken Token { get; set; }

        public bool IsSubscriptionActive { get; set; }

        public void When(SubscriberSubscribedForFireBase e)
        {
            Id = e.Id;
            UserId = e.SubscriberId;
            Token = e.Token;
            IsSubscriptionActive = true;
        }

        public void When(SubscriberUnSubscribedFromFireBase e)
        {
            IsSubscriptionActive = false;
        }
    }
}
