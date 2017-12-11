using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_subscribed_twice_with_different_token
    {
        Establish context = () =>
        {
            var id = new SubscriptionId("id", "elders");
            projection = new SubscriberTokensProjection();
            firstSubscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            secondSubscriptionToken = new SubscriptionToken("token2", SubscriptionType.FireBase);
            var subscriberId = new SubscriberId("kv", "elders");
            var firstEvent = new Subscribed(id, subscriberId, firstSubscriptionToken);
            secondEvent = new Subscribed(id, subscriberId, secondSubscriptionToken);
            projection.Handle(firstEvent);
        };

        Because of = () => projection.Handle(secondEvent);

        It should_have_correct_number_of_subscriptions = () => projection.State.Tokens.Count.ShouldEqual(2);

        It should_have_correct_first_token = () => projection.State.Tokens.ShouldContain(firstSubscriptionToken);

        It should_have_correct_second_token = () => projection.State.Tokens.ShouldContain(secondSubscriptionToken);

        static SubscriberTokensProjection projection;
        static Subscribed secondEvent;
        static SubscriptionToken firstSubscriptionToken;
        static SubscriptionToken secondSubscriptionToken;
    }
}
