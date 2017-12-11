using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class SubscriptionState : AggregateRootState<Subscription, SubscriptionId>
    {
        public override SubscriptionId Id { get; set; }

        public SubscriberId SubscriberId { get; set; }

        public SubscriptionToken SubscriptionToken { get; set; }

        public SubscriptionType SubscriptionType { get; set; }


        public bool IsSubscriptionActive { get; set; }

        public void When(Subscribed e)
        {
            Id = e.Id;
            SubscriberId = e.SubscriberId;
            SubscriptionToken = e.SubscriptionToken;
            IsSubscriptionActive = true;
        }

        public void When(UnSubscribed e)
        {
            IsSubscriptionActive = false;
        }
    }
}
