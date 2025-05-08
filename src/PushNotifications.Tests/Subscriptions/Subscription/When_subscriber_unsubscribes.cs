using Machine.Specifications;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;
using System;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(DeviceSubscription))]
    public class When_subscriber_unsubscribes
    {
        Establish context = () =>
        {
            id = new DeviceSubscriptionId("elders", "id");
            subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            ar = new DeviceSubscription(id, subscriberId, subscriptionToken);
        };

        Because of = () => ar.UnSubscribe(subscriberId, DateTimeOffset.UtcNow);

        It should_unsubscribe = () => ar.ShouldHaveEvent<UnSubscribed>(e =>
        {
            e.Id.ShouldEqual(id);
            e.SubscriberId.ShouldEqual(subscriberId);
            e.SubscriptionToken.ShouldEqual(subscriptionToken);
        });

        It should_have_not_active_subscription = () => ar.RootState().IsSubscriptionActive.ShouldBeFalse();

        static DeviceSubscription ar;
        static DeviceSubscriptionId id;
        static DeviceSubscriberId subscriberId;
        static SubscriptionToken subscriptionToken;
    }
}
