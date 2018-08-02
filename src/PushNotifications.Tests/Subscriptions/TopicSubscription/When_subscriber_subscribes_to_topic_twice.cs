using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(TopicSubscription))]
    public class When_subscriber_subscribes_to_topic_twice
    {
        Establish context = () =>
        {
            subscriberId = new SubscriberId("kv", "elders");
            topic = new Topic("topic");
            id = new TopicSubscriptionId(subscriberId, topic, "elders");
            ar = new TopicSubscription(id);
        };
        Because of = () => ar.SubscribeToTopic(id);

        It should_not_raise_an_event = () => ar.ShouldHaveEventsCount<SubscribedToTopic>(1);

        static TopicSubscription ar;
        static Topic topic;
        static TopicSubscriptionId id;
        static SubscriberId subscriberId;
    }
}
