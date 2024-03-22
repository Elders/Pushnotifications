using Machine.Specifications;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;
using System;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_subscribed_twice_with_same_token
    {
        Establish context = () =>
        {
            var id = DeviceSubscriptionId.New("elders", "id");
            projection = new SubscriberTokensProjection();
            var subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            var subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            @event = new Subscribed(id, subscriberId, subscriptionToken, DateTimeOffset.UtcNow);
            projection.HandleAsync(@event);
        };

        Because of = () => projection.HandleAsync(@event);

        It should_have_correct_number_of_subscriptions = () => projection.State.Tokens.Count.ShouldEqual(1);

        static SubscriberTokensProjection projection;
        static Subscribed @event;
    }
}
