using Machine.Specifications;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(TopicsPerSubscriberProjection))]
    public class When_subscriber_has_unsubscribed_from_topic
    {
        Establish context = () =>
        {
            topic = new Topic("topic");
            subscriberId = new DeviceSubscriberId("elders", "kv", "app");
            var id = new TopicSubscriptionId("elders", topic, subscriberId);

            projection = new TopicsPerSubscriberProjection();
            @event = new SubscribedToTopic(id);
            projection.HandleAsync(@event);

            unsubscribeEvent = new UnsubscribedFromTopic(id);
        };

        Because of = () => projection.HandleAsync(unsubscribeEvent);

        It should_have_correct_id = () => projection.State.SubscriberId.ShouldEqual(subscriberId);
        It should_not_have_any_topics_registered = () => projection.State.Topics.ShouldBeEmpty();

        static TopicsPerSubscriberProjection projection;
        static DeviceSubscriberId subscriberId;
        static Topic topic;
        static SubscribedToTopic @event;
        static UnsubscribedFromTopic unsubscribeEvent;
    }
}
