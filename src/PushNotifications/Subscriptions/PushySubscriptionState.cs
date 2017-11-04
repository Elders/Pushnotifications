using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushySubscriptions;
using PushNotifications.Contracts.PushySubscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class PushySubscriptionState : AggregateRootState<PushySubscription, PushySubscriptionId>
    {
        public override PushySubscriptionId Id { get; set; }

        public SubscriberId UserId { get; set; }

        public SubscriptionToken Token { get; set; }

        public bool IsSubscriptionActive { get; set; }

        public void When(SubscriberSubscribedForPushy e)
        {
            Id = e.Id;
            UserId = e.SubscriberId;
            Token = e.Token;
            IsSubscriptionActive = true;
        }

        public void When(SubscriberUnSubscribedFromPushy e)
        {
            IsSubscriptionActive = false;
        }
    }
}
