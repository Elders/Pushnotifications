using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(Subscription))]
    public class When_subscription_subscriber_change
    {
        Establish context = () =>
        {
            id = new SubscriptionId("id", "elders");
            subscriberId = new SubscriberId("kv", "elders");
            newSubscriberId = new SubscriberId("kv2", "elders");
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
