using Machine.Specifications;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_subscribed_twice_with_different_token
    {
        Establish context = () =>
        {
            var id = DeviceSubscriptionId.New("elders", "id");
            projection = new SubscriberTokensProjection();
            firstSubscriptionToken = new SubscriptionToken("token", SubscriptionType.FireBase);
            secondSubscriptionToken = new SubscriptionToken("token2", SubscriptionType.FireBase);
            var subscriberId = new DeviceSubscriberId("kv", "elders", "app");
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
