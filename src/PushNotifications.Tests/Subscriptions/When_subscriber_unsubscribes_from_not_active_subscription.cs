using System.Linq;
using Elders.Cronus.DomainModeling;
using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(Subscription))]
    public class When_subscriber_unsubscribes_from_not_active_subscription
    {
        Establish context = () =>
        {
            id = new SubscriptionId("id", "elders");
            subscriberId = new SubscriberId("kv", "elders");
            subscriptionToken = new SubscriptionToken("token");
            subscriptionType = SubscriptionType.FireBase;
            ar = new Subscription(id, subscriberId, subscriptionToken, subscriptionType);
            ar.UnSubscribe(subscriberId);
        };

        Because of = () => ar.UnSubscribe(subscriberId);

        It should_unsubscribe = () => ar.ShouldHaveEvent<UnSubscribed>(e =>
        {
            e.Id.ShouldEqual(id);
            e.SubscriberId.ShouldEqual(subscriberId);
            e.SubscriptionToken.ShouldEqual(subscriptionToken);
            e.SubscriptionType.ShouldEqual(subscriptionType);
        });

        It should_be_no_change = () => ((IAggregateRoot)ar).UncommittedEvents.Count().ShouldEqual(2);

        static Subscription ar;
        static SubscriptionId id;
        static SubscriberId subscriberId;
        static SubscriptionToken subscriptionToken;
        static SubscriptionType subscriptionType;
    }
}
