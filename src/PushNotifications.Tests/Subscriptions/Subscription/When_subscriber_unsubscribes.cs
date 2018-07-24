using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(Subscription))]
    public class When_subscriber_unsubscribes
    {
        Establish context = () =>
        {
            id = new SubscriptionId("id", "elders");
            subscriberId = new SubscriberId("kv", "elders");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            ar = new Subscription(id, subscriberId, subscriptionToken);
        };

        Because of = () => ar.UnSubscribe(subscriberId);

        It should_unsubscribe = () => ar.ShouldHaveEvent<UnSubscribed>(e =>
        {
            e.Id.ShouldEqual(id);
            e.SubscriberId.ShouldEqual(subscriberId);
            e.SubscriptionToken.ShouldEqual(subscriptionToken);
        });

        It should_have_not_active_subscription = () => ar.RootState().IsSubscriptionActive.ShouldBeFalse();

        static Subscription ar;
        static SubscriptionId id;
        static SubscriberId subscriberId;
        static SubscriptionToken subscriptionToken;
    }
}
