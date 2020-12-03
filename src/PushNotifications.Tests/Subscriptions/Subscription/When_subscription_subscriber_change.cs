using Machine.Specifications;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(Subscription))]
    public class When_subscription_subscriber_change
    {
        Establish context = () =>
        {
            id = SubscriptionId.New("elders", "id");
            subscriberId = new SubscriberId("kv", "elders", "app");
            newSubscriberId = new SubscriberId("kv2", "elders", "app");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            ar = new Subscription(id, subscriberId, subscriptionToken);
        };

        Because of = () => ar.Subscribe(newSubscriberId);

        It should_have_correct_new_subscriber = () => ar.ShouldHaveEvent<Subscribed>(e =>
        {
            e.Id.ShouldEqual(id);
            e.SubscriberId.ShouldEqual(newSubscriberId);
            e.SubscriptionToken.ShouldEqual(subscriptionToken);
        });

        static Subscription ar;
        static SubscriptionId id;
        static SubscriberId subscriberId;
        static SubscriberId newSubscriberId;
        static SubscriptionToken subscriptionToken;
    }
}
