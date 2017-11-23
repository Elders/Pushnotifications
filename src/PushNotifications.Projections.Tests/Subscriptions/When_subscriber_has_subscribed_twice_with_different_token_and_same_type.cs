using System.Linq;
using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_subscribed_twice_with_different_token_and_same_type
    {
        Establish context = () =>
        {
            var id = new SubscriptionId("id", "elders");
            projection = new SubscriberTokensProjection();
            firstSubscriptionToken = new SubscriptionToken("token");
            secondSubscriptionToken = new SubscriptionToken("token2");
            subscriberId = new SubscriberId("kv", "elders");
            subscriptionType = SubscriptionType.FireBase;
            firstEvet = new Subscribed(id, subscriberId, firstSubscriptionToken, subscriptionType);
            projection.Handle(firstEvet);

            secondEvent = new Subscribed(id, subscriberId, secondSubscriptionToken, subscriptionType);
        };

        Because of = () => projection.Handle(secondEvent);

        It should_have_correct_number_of_subscriptions = () => projection.State.TokenTypePairs.Count.ShouldEqual(2);

        It should_have_correct_number_of_subscriptions_with_first_token = () => projection.State.TokenTypePairs.Where(x => x.SubscriptionToken == firstSubscriptionToken).Count().ShouldEqual(1);

        It should_have_correct_number_of_subscriptions_with_second_token = () => projection.State.TokenTypePairs.Where(x => x.SubscriptionToken == secondSubscriptionToken).Count().ShouldEqual(1);

        static SubscriberTokensProjection projection;
        static SubscriberId subscriberId;
        static SubscriptionToken firstSubscriptionToken;
        static SubscriptionToken secondSubscriptionToken;
        static SubscriptionType subscriptionType;
        static Subscribed firstEvet;
        static Subscribed secondEvent;
    }
}
