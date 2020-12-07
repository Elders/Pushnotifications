using Machine.Specifications;
using PushNotifications.Subscriptions;
using System.Linq;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(DeviceSubscription))]
    public class When_subscriber_different_then_original_one_subscribes_to_not_active_subscription
    {
        Establish context = () =>
        {
            id = DeviceSubscriptionId.New("elders", "id");
            subscriberId = new DeviceSubscriberId("kv", "elders", "app");
            newSubscriberId = new DeviceSubscriberId("kv2", "elders", "app");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            ar = new DeviceSubscription(id, subscriberId, subscriptionToken);
            ar.UnSubscribe(subscriberId);
        };

        Because of = () => ar.Subscribe(newSubscriberId);

        It should_activate = () => ar.RootState().IsSubscriptionActive.ShouldBeTrue();

        It should_have_correct_new_subscriber = () => ar.RootState().Subscribers.First().ShouldEqual(newSubscriberId);

        static DeviceSubscription ar;
        static DeviceSubscriptionId id;
        static DeviceSubscriberId subscriberId;
        static DeviceSubscriberId newSubscriberId;
        static SubscriptionToken subscriptionToken;
    }
}
