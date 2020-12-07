using Machine.Specifications;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(DeviceSubscription))]
    public class When_subscriber_subscribes_to_not_active_subscription
    {
        Establish context = () =>
        {
            id = DeviceSubscriptionId.New("elders", "id");
            subscriberId = new DeviceSubscriberId("kv", "elders", "app");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            ar = new DeviceSubscription(id, subscriberId, subscriptionToken);
            ar.UnSubscribe(subscriberId);
        };

        Because of = () => ar.Subscribe(subscriberId);

        It should_activate = () => ar.RootState().IsSubscriptionActive.ShouldBeTrue();

        static DeviceSubscription ar;
        static DeviceSubscriptionId id;
        static DeviceSubscriberId subscriberId;
        static SubscriptionToken subscriptionToken;
    }
}
