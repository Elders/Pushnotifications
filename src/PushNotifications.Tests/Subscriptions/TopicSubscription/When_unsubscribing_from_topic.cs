using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(TopicSubscription))]
    public class When_unsubscribing_from_topic
    {
        Establish context = () =>
        {
            var tenant = "elders";
            topic = new Topic("topic");
            subscriberId = new SubscriberId("id", tenant);
            topicSubscriptionId = new TopicSubscriptionId(subscriberId, topic, tenant);
            aggregate = new TopicSubscription(topicSubscriptionId);
        };

        Because of = () => aggregate.UnsubscribeFromTopic(topicSubscriptionId);

        It should = () => aggregate.RootState().IsSubscriptionActive.ShouldBeFalse();

        It should_have_the_same_topic = () => aggregate.RootState().Id.Topic.ShouldEqual(topic);

        It should_have_the_same_subscriber_id = () => aggregate.RootState().Id.SubscriberId.ShouldEqual(subscriberId);

        It should_have_the_same_topic_subscription_id = () => aggregate.RootState().Id.ShouldEqual(topicSubscriptionId);

        static TopicSubscription aggregate;
        static TopicSubscriptionId topicSubscriptionId;
        static SubscriberId subscriberId;
        static Topic topic;
    }
}
