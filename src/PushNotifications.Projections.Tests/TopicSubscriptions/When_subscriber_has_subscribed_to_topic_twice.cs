using Machine.Specifications;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(TopicsPerSubscriberProjection))]
    public class When_subscriber_has_subscribed_to_topic_twice
    {
        Establish context = () =>
        {
            var topic = new Topic("topic");
            var subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            var id = new TopicSubscriptionId("elders", topic, subscriberId);

            projection = new TopicsPerSubscriberProjection();
            @event = new SubscribedToTopic(id);
            projection.HandleAsync(@event);
        };

        Because of = () => projection.HandleAsync(@event);

        It should_have_exactly_one_topic_subscription = () => projection.State.Topics.Count.ShouldEqual(1);

        static TopicsPerSubscriberProjection projection;
        static SubscribedToTopic @event;
    }
}
