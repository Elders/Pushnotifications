using Machine.Specifications;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;
using System;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(TopicSubscription))]
    public class When_unsubscribing_from_topic_and_subscribing_again
    {
        Establish context = () =>
        {
            var tenant = "elders";
            topic = new Topic("topic");
            subscriberId = new DeviceSubscriberId(tenant, "id", "app");
            topicSubscriptionId = new TopicSubscriptionId(tenant, topic, subscriberId);
            aggregate = new TopicSubscription(topicSubscriptionId);
            aggregate.UnsubscribeFromTopic(topicSubscriptionId, DateTimeOffset.UtcNow);
        };

        Because of = () => aggregate.SubscribeToTopic(topicSubscriptionId, DateTimeOffset.UtcNow);

        It should = () => aggregate.RootState().IsSubscriptionActive.ShouldBeTrue();

        It should_have_the_same_topic = () => aggregate.RootState().Id.Topic.ShouldEqual(topic);

        It should_have_the_same_subscriber_id = () => aggregate.RootState().Id.SubscriberId.ShouldEqual(subscriberId);

        It should_have_the_same_topic_subscription_id = () => aggregate.RootState().Id.ShouldEqual(topicSubscriptionId);

        It should_have_two_topic_subscribed_events = () => aggregate.ShouldHaveEventsCount<SubscribedToTopic>(2);

        static TopicSubscription aggregate;
        static TopicSubscriptionId topicSubscriptionId;
        static DeviceSubscriberId subscriberId;
        static Topic topic;
    }
}
