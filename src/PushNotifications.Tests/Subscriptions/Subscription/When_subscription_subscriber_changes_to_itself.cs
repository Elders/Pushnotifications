using System.Linq;
using Elders.Cronus;
using Machine.Specifications;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(Subscription))]
    public class When_subscription_subscriber_changes_to_itself
    {
        Establish context = () =>
        {
            id = SubscriptionId.New("elders", "id");
            subscriberId = new SubscriberId("kv", "elders", "app");
            newSubscriberIdButStillTheSame = new SubscriberId("kv", "elders", "app");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);

            ar = new Subscription(id, subscriberId, subscriptionToken);
        };

        Because of = () => ar.Subscribe(newSubscriberIdButStillTheSame);

        It should_be_no_change = () => ((IAggregateRoot)ar).UncommittedEvents.Count().ShouldEqual(1);

        static Subscription ar;
        static SubscriptionId id;
        static SubscriberId subscriberId;
        static SubscriberId newSubscriberIdButStillTheSame;
        static SubscriptionToken subscriptionToken;
    }
}
