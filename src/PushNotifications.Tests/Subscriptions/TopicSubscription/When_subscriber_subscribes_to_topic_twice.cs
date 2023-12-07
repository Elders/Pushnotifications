using Machine.Specifications;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;
using System;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(TopicSubscription))]
    public class When_subscriber_subscribes_to_topic_twice
    {
        Establish context = () =>
        {
            subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            topic = new Topic("topic");
            id = new TopicSubscriptionId("elders", topic, subscriberId);
            ar = new TopicSubscription(id);
        };
        Because of = () => ar.SubscribeToTopic(id, DateTimeOffset.UtcNow);

        It should_not_raise_an_event = () => ar.ShouldHaveEventsCount<SubscribedToTopic>(1);

        static TopicSubscription ar;
        static Topic topic;
        static TopicSubscriptionId id;
        static DeviceSubscriberId subscriberId;
    }
}
