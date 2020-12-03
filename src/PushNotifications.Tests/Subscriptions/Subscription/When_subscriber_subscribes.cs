using Elders.Cronus;
using Machine.Specifications;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(Subscription))]
    public class When_subscriber_subscribes
    {
        Establish context = () =>
        {
            id = SubscriptionId.New("elders", "id");
            subscriberId = new SubscriberId("kv", "elders", "app");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
        };

        Because of = () => ar = new Subscription(id, subscriberId, subscriptionToken);

        It should_create_subscription = () => ar.ShouldHaveEvent<Subscribed>(e =>
        {
            e.Id.ShouldEqual(id);
            e.SubscriberId.ShouldEqual(subscriberId);
            e.SubscriptionToken.ShouldEqual(subscriptionToken);
        });

        static IAggregateRoot ar;
        static SubscriptionId id;
        static SubscriberId subscriberId;
        static SubscriptionToken subscriptionToken;
    }
}
