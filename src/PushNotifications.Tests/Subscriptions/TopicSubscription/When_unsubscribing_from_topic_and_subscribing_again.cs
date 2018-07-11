using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(TopicSubscription))]
    public class When_unsubscribing_from_topic_and_subscribing_again
    {
        Establish context = () =>
        {
            var tenant = "elders";
            topic = new Topic("topic");
            subscriberId = new SubscriberId("id", tenant);
            topicSubscriptionId = new TopicSubscriptionId(subscriberId, topic, tenant);
            subscriptionType = SubscriptionType.FireBase;
            aggregate = new TopicSubscription(topicSubscriptionId, subscriptionType);
            aggregate.UnsubscribeFromTopic(topicSubscriptionId, subscriptionType);
        };

        Because of = () => aggregate.SubscribeToTopic(topicSubscriptionId, subscriptionType);

        It should = () => aggregate.RootState().IsSubscriptionActive.ShouldBeTrue();

        It should_have_the_same_topic = () => aggregate.RootState().Id.Topic.ShouldEqual(topic);

        It should_have_the_same_subscriber_id = () => aggregate.RootState().Id.SubscriberId.ShouldEqual(subscriberId);

        It should_have_the_same_topic_subscription_id = () => aggregate.RootState().Id.ShouldEqual(topicSubscriptionId);

        It should_have_two_topic_subscribed_events = () => aggregate.ShouldHaveEventsCount<Contracts.Subscriptions.Events.SubscribedToTopic>(2);

        static TopicSubscription aggregate;
        static TopicSubscriptionId topicSubscriptionId;
        static SubscriberId subscriberId;
        static SubscriptionType subscriptionType;
        static Topic topic;
    }
}
