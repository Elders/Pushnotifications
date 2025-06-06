﻿using System;
using System.Linq;
using Elders.Cronus;
using Elders.Cronus.Projections;
using Machine.Specifications;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_subscribed
    {
        Establish context = () =>
        {
            var id = new DeviceSubscriptionId("elders", "id");
            projection = new SubscriberTokensProjection();
            subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            @event = new Subscribed(id, subscriberId, subscriptionToken, DateTimeOffset.UtcNow);
        };

        Because of = () => projection.HandleAsync(@event);

        It should_subscribe_for_the_event = () => ((IProjectionDefinition)projection).GetProjectionIds(@event).ShouldContain(subscriberId);
        It should_handle_the_event = () => typeof(IEventHandler<Subscribed>).IsAssignableFrom(projection.GetType()).ShouldBeTrue();

        It should_have_correct_id = () => projection.State.SubscriberId.ShouldEqual(subscriberId);
        It should_have_valid_subscription_tokens = () => projection.State.Tokens.ShouldNotBeNull();
        It should_have_correct_subscription_token = () => projection.State.Tokens.Single().ShouldEqual(subscriptionToken);

        static SubscriberTokensProjection projection;
        static DeviceSubscriberId subscriberId;
        static SubscriptionToken subscriptionToken;
        static Subscribed @event;
    }
}
