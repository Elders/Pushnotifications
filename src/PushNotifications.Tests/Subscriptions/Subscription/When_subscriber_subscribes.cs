using Elders.Cronus;
using Machine.Specifications;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(DeviceSubscription))]
    public class When_subscriber_subscribes
    {
        Establish context = () =>
        {
            id = new DeviceSubscriptionId("elders", "id");
            subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
        };

        Because of = () => ar = new DeviceSubscription(id, subscriberId, subscriptionToken);

        It should_create_subscription = () => ar.ShouldHaveEvent<Subscribed>(e =>
        {
            e.Id.ShouldEqual(id);
            e.SubscriberId.ShouldEqual(subscriberId);
            e.SubscriptionToken.ShouldEqual(subscriptionToken);
        });

        static IAggregateRoot ar;
        static DeviceSubscriptionId id;
        static DeviceSubscriberId subscriberId;
        static SubscriptionToken subscriptionToken;
    }
}
