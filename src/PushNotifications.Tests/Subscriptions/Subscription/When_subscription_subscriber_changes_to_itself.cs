using System;
using System.Linq;
using Elders.Cronus;
using Machine.Specifications;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(DeviceSubscription))]
    public class When_subscription_subscriber_changes_to_itself
    {
        Establish context = () =>
        {
            id = DeviceSubscriptionId.New("elders", "id");
            subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            newSubscriberIdButStillTheSame = new DeviceSubscriberId("elders", "kv", "app");
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);

            ar = new DeviceSubscription(id, subscriberId, subscriptionToken);
        };

        Because of = () => ar.Subscribe(newSubscriberIdButStillTheSame, DateTimeOffset.UtcNow);

        It should_be_no_change = () => ((IAggregateRoot)ar).UncommittedEvents.Count().ShouldEqual(1);

        static DeviceSubscription ar;
        static DeviceSubscriptionId id;
        static DeviceSubscriberId subscriberId;
        static DeviceSubscriberId newSubscriberIdButStillTheSame;
        static SubscriptionToken subscriptionToken;
    }
}
