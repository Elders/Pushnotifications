using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_subscribed_twice_with_same_token_and_type
    {
        Establish context = () =>
        {
            var id = new SubscriptionId("id", "elders");
            projection = new SubscriberTokensProjection();
            var subscriptionToken = new SubscriptionToken("token");
            var subscriberId = new SubscriberId("kv", "elders");
            var subscriptionType = SubscriptionType.FireBase;
            @event = new Subscribed(id, subscriberId, subscriptionToken, subscriptionType);
            projection.Handle(@event);
        };

        Because of = () => projection.Handle(@event);

        It should_have_correct_number_of_subscriptions = () => projection.State.TokenTypePairs.Count.ShouldEqual(1);

        static SubscriberTokensProjection projection;
        static Subscribed @event;
    }
}
