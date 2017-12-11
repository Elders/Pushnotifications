using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_subscribed_twice_with_same_token
    {
        Establish context = () =>
        {
            var id = new SubscriptionId("id", "elders");
            projection = new SubscriberTokensProjection();
            var subscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            var subscriberId = new SubscriberId("kv", "elders");
            @event = new Subscribed(id, subscriberId, subscriptionToken);
            projection.Handle(@event);
        };

        Because of = () => projection.Handle(@event);

        It should_have_correct_number_of_subscriptions = () => projection.State.Tokens.Count.ShouldEqual(1);

        static SubscriberTokensProjection projection;
        static Subscribed @event;
    }
}
