using System.Linq;
using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_subscribed_twice_with_same_token_and_different_type
    {
        Establish context = () =>
        {
            var id = new SubscriptionId("id", "elders");
            projection = new SubscriberTokensProjection();
            subscriptionToken = new SubscriptionToken("token");
            subscriberId = new SubscriberId("kv", "elders");
            firstSubscriptionType = SubscriptionType.FireBase;
            secondSubscriptionType = SubscriptionType.Pushy;
            firstEvet = new Subscribed(id, subscriberId, subscriptionToken, firstSubscriptionType);
            projection.Handle(firstEvet);

            secondEvent = new Subscribed(id, subscriberId, subscriptionToken, secondSubscriptionType);
        };

        Because of = () => projection.Handle(secondEvent);

        It should_have_correct_number_of_subscriptions = () => projection.State.TokenTypePairs.Count.ShouldEqual(2);

        It should_have_correct_number_of_subscriptions_from_first_type = () => projection.State.TokenTypePairs.Where(x => x.SubscriptionType == firstSubscriptionType).Count().ShouldEqual(1);

        It should_have_correct_number_of_subscriptions_from_second_type = () => projection.State.TokenTypePairs.Where(x => x.SubscriptionType == secondSubscriptionType).Count().ShouldEqual(1);

        static SubscriberTokensProjection projection;
        static SubscriberId subscriberId;
        static SubscriptionToken subscriptionToken;
        static SubscriptionType firstSubscriptionType;
        static SubscriptionType secondSubscriptionType;
        static Subscribed firstEvet;
        static Subscribed secondEvent;
    }
}
