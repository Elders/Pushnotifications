using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(Subscription))]
    public class When_subscriber_subscribes_to_not_active_subscription
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

        Because of = () => ar.Subscribe(subscriberId);

        It should_activate = () => ar.RootState().IsSubscriptionActive.ShouldBeTrue();

        static Subscription ar;
        static SubscriptionId id;
        static SubscriberId subscriberId;
        static SubscriptionToken subscriptionToken;
        static SubscriptionType subscriptionType;
    }
}
