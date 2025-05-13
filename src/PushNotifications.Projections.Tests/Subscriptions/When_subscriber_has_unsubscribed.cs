using Elders.Cronus;
using Elders.Cronus.Projections;
using Machine.Specifications;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;
using System;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_unsubscribed
    {
        Establish context = () =>
        {
            var id = new DeviceSubscriptionId("elders", "id");
            projection = new SubscriberTokensProjection();
            var subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            subscribedEvent = new Subscribed(id, subscriberId, subscriptionToken, DateTimeOffset.UtcNow);
            unSubscribedEvent = new UnSubscribed(id, subscriberId, subscriptionToken, DateTimeOffset.UtcNow);
            projection.HandleAsync(subscribedEvent);
        };

        Because of = () => projection.HandleAsync(unSubscribedEvent);

        It should_subscribe_for_the_event = () => ((IProjectionDefinition)projection).GetProjectionIds(unSubscribedEvent).ShouldContain(subscriberId);
        It should_handle_the_event = () => typeof(IEventHandler<UnSubscribed>).IsAssignableFrom(projection.GetType()).ShouldBeTrue();

        It should_have_correct_id = () => projection.State.SubscriberId.ShouldEqual(subscriberId);
        It should_have_correct_zero_subscriptions = () => projection.State.Tokens.Count.ShouldEqual(0);

        static SubscriberTokensProjection projection;
        static DeviceSubscriberId subscriberId;

        static Subscribed subscribedEvent;
        static UnSubscribed unSubscribedEvent;
    }
}
