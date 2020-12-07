using System.Linq;
using Elders.Cronus;
using Machine.Specifications;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(DeviceSubscription))]
    public class When_subscriber_unsubscribes_from_not_active_subscription
    {
        Establish context = () =>
        {
            id = DeviceSubscriptionId.New("elders", "id");
            subscriberId = new DeviceSubscriberId("kv", "elders", "app");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            ar = new DeviceSubscription(id, subscriberId, subscriptionToken);
            ar.UnSubscribe(subscriberId);
        };

        Because of = () => ar.UnSubscribe(subscriberId);

        It should_unsubscribe = () => ar.ShouldHaveEvent<UnSubscribed>(e =>
        {
            e.Id.ShouldEqual(id);
            e.SubscriberId.ShouldEqual(subscriberId);
            e.SubscriptionToken.ShouldEqual(subscriptionToken);
        });

        It should_be_no_change = () => ((IAggregateRoot)ar).UncommittedEvents.Count().ShouldEqual(2);

        static DeviceSubscription ar;
        static DeviceSubscriptionId id;
        static DeviceSubscriberId subscriberId;
        static SubscriptionToken subscriptionToken;
    }
}
