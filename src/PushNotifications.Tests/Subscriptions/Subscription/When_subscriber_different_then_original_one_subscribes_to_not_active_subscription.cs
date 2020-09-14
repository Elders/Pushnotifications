using Machine.Specifications;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(Subscription))]
    public class When_subscriber_different_then_original_one_subscribes_to_not_active_subscription
    {
        Establish context = () =>
        {
            id = SubscriptionId.New("elders", "id");
            subscriberId = new SubscriberId("kv", "elders");
            newSubscriberId = new SubscriberId("kv2", "elders");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            ar = new Subscription(id, subscriberId, subscriptionToken);
            ar.UnSubscribe(subscriberId);
        };

        Because of = () => ar.Subscribe(newSubscriberId);

        It should_activate = () => ar.RootState().IsSubscriptionActive.ShouldBeTrue();

        It should_have_correct_new_subscriber = () => ar.RootState().SubscriberId.ShouldEqual(newSubscriberId);

        static Subscription ar;
        static SubscriptionId id;
        static SubscriberId subscriberId;
        static SubscriberId newSubscriberId;
        static SubscriptionToken subscriptionToken;
    }
}
