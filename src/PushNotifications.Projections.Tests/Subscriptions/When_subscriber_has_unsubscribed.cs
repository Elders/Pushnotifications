﻿using Elders.Cronus;
using Elders.Cronus.Projections;
using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_unsubscribed
    {
        Establish context = () =>
        {
            var id = new SubscriptionId("id", "elders");
            projection = new SubscriberTokensProjection();
            var subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            subscriberId = new SubscriberId("kv", "elders");
            subscribedEvent = new Subscribed(id, subscriberId, subscriptionToken);
            unSubscribedEvent = new UnSubscribed(id, subscriberId, subscriptionToken);
            projection.Handle(subscribedEvent);
        };

        Because of = () => projection.Handle(unSubscribedEvent);

        It should_subscribe_for_the_event = () => ((IProjectionDefinition)projection).GetProjectionIds(unSubscribedEvent).ShouldContain(subscriberId);
        It should_handle_the_event = () => typeof(IEventHandler<UnSubscribed>).IsAssignableFrom(projection.GetType()).ShouldBeTrue();

        It should_have_correct_id = () => projection.State.SubscriberId.ShouldEqual(subscriberId);
        It should_have_correct_zero_subscriptions = () => projection.State.Tokens.Count.ShouldEqual(0);

        static SubscriberTokensProjection projection;
        static SubscriberId subscriberId;

        static Subscribed subscribedEvent;
        static UnSubscribed unSubscribedEvent;
    }
}
