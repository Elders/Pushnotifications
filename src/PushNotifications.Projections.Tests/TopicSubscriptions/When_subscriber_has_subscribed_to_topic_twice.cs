using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(TopicsPerSubscriberProjection))]
    public class When_subscriber_has_subscribed_to_topic_twice
    {
        Establish context = () =>
        {
            var topic = new Topic("topic");
            var subscriberId = new SubscriberId("kv", "elders");
            var id = new TopicSubscriptionId(subscriberId, topic, "elders");

            projection = new TopicsPerSubscriberProjection();
            @event = new SubscribedToTopic(id, SubscriptionType.FireBase);
            projection.Handle(@event);
        };

        Because of = () => projection.Handle(@event);

        It should_have_exactly_one_topic_subscription = () => projection.State.Topics.Count.ShouldEqual(1);

        static TopicsPerSubscriberProjection projection;
        static SubscribedToTopic @event;
    }
}
