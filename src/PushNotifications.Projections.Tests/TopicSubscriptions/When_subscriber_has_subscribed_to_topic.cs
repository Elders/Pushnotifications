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
    [Subject(nameof(TopicsPerSubscriberProjection))]
    public class When_subscriber_has_subscribed_to_topic
    {
        Establish context = () =>
        {
            topic = new Topic("topic");
            subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            var id = new TopicSubscriptionId("elders", topic, subscriberId);

            projection = new TopicsPerSubscriberProjection();
            @event = new SubscribedToTopic(id, DateTimeOffset.UtcNow);
        };

        Because of = () => projection.HandleAsync(@event);

        It should_subscribe_for_the_event = () => ((IProjectionDefinition)projection).GetProjectionIds(@event).ShouldContain(subscriberId);
        It should_handle_the_event = () => typeof(IEventHandler<SubscribedToTopic>).IsAssignableFrom(projection.GetType()).ShouldBeTrue();

        It should_have_correct_id = () => projection.State.SubscriberId.ShouldEqual(subscriberId);
        It should_have_valid_subscription_tokens = () => projection.State.Topics.ShouldNotBeNull();
        It should_have_correct_subscription_token = () => projection.State.Topics.Single().ShouldEqual(topic);

        static TopicsPerSubscriberProjection projection;
        static DeviceSubscriberId subscriberId;
        static Topic topic;
        static SubscribedToTopic @event;
    }
}
