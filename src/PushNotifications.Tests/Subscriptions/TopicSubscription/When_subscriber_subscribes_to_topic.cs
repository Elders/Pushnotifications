﻿using Elders.Cronus;
using Machine.Specifications;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(TopicSubscription))]
    public class When_subscriber_subscribes_to_topic
    {
        Establish context = () =>
        {
            subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            topic = new Topic("topic");
            id = new TopicSubscriptionId("elders", topic, subscriberId);
        };
        Because of = () => ar = new TopicSubscription(id);

        It should_create_subscription = () => ar.ShouldHaveEvent<SubscribedToTopic>(e =>
        {
            e.Id.ShouldEqual(id);
            e.Id.SubscriberId.ShouldEqual(subscriberId);
            e.Id.Topic.ShouldEqual(topic);
        });

        static IAggregateRoot ar;
        static Topic topic;
        static TopicSubscriptionId id;
        static DeviceSubscriberId subscriberId;
    }
}
