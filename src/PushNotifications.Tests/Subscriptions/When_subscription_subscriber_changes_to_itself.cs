using System.Linq;
using Elders.Cronus.DomainModeling;
using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(Subscription))]
    public class When_subscription_subscriber_changes_to_itself
    {
        Establish context = () =>
        {
            id = new SubscriptionId("id", "elders");
            subscriberId = new SubscriberId("kv", "elders");
            newSubscriberIdButStillTheSame = new SubscriberId("kv", "elders");
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
