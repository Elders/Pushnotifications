using System.Linq;
using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_unsubscribed_to_one_subscription_after_having_two_subscriptions
    {
        Establish context = () =>
        {
            var id = new SubscriptionId("id", "elders");
            projection = new SubscriberTokensProjection();
            var firstSubscriptionToken = new SubscriptionToken("token1", SubscriptionType.FireBase);
            secondSubscriptionToken = new SubscriptionToken("token2", SubscriptionType.FireBase);

            subscriberId = new SubscriberId("kv", "elders");
            var firstSubscribedEvent = new Subscribed(id, subscriberId, firstSubscriptionToken);
            var secondSubscribedEvent = new Subscribed(id, subscriberId, secondSubscriptionToken);
            unSubscribedEvent = new UnSubscribed(id, subscriberId, firstSubscriptionToken);
            projection.Handle(firstSubscribedEvent);
            projection.Handle(secondSubscribedEvent);
        };

        Because of = () => projection.Handle(unSubscribedEvent);

        It should_have_one_subscription = () => projection.State.Tokens.Count.ShouldEqual(1);

        It should_have_correct_subscription = () => projection.State.Tokens.Single().ShouldEqual(secondSubscriptionToken);

        static SubscriberTokensProjection projection;
        static SubscriberId subscriberId;
        static UnSubscribed unSubscribedEvent;
        static SubscriptionToken secondSubscriptionToken;
    }
}
