﻿using Machine.Specifications;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(TopicsPerSubscriberProjection))]
    public class When_subscriber_has_unsubscribed_from_topic_twice
    {
        Establish context = () =>
        {
            topic = new Topic("topic");
            subscriberId = new SubscriberId("kv", "elders");
            var id = new TopicSubscriptionId(subscriberId, topic, "elders");

            projection = new TopicsPerSubscriberProjection();
            @event = new SubscribedToTopic(id);
            projection.Handle(@event);

            unsubscribeEvent = new UnsubscribedFromTopic(id);
            projection.Handle(unsubscribeEvent);
        };

        Because of = () => projection.Handle(unsubscribeEvent);

        It should_have_correct_id = () => projection.State.SubscriberId.ShouldEqual(subscriberId);
        It should_not_have_any_topics_registered = () => projection.State.Topics.ShouldBeEmpty();

        static TopicsPerSubscriberProjection projection;
        static SubscriberId subscriberId;
        static Topic topic;
        static SubscribedToTopic @event;
        static UnsubscribedFromTopic unsubscribeEvent;
    }
}
