using Machine.Specifications;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;
using System;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_subscribed_twice_with_different_token
    {
        Establish context = () =>
        {
            var id = DeviceSubscriptionId.New("elders", "id");
            projection = new SubscriberTokensProjection();
            firstSubscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            secondSubscriptionToken = new SubscriptionToken("token2", SubscriptionType.FireBase);
            var subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            var firstEvent = new Subscribed(id, subscriberId, firstSubscriptionToken, DateTimeOffset.UtcNow);
            secondEvent = new Subscribed(id, subscriberId, secondSubscriptionToken, DateTimeOffset.UtcNow);
            projection.HandleAsync(firstEvent);
        };

        Because of = () => projection.HandleAsync(secondEvent);

        It should_have_correct_number_of_subscriptions = () => projection.State.Tokens.Count.ShouldEqual(2);

        It should_have_correct_first_token = () => projection.State.Tokens.ShouldContain(firstSubscriptionToken);

        It should_have_correct_second_token = () => projection.State.Tokens.ShouldContain(secondSubscriptionToken);

        static SubscriberTokensProjection projection;
        static Subscribed secondEvent;
        static SubscriptionToken firstSubscriptionToken;
        static SubscriptionToken secondSubscriptionToken;
    }
}
