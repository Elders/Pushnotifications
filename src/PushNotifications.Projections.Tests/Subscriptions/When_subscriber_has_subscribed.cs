using System.Linq;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(SubscriberTokensProjection))]
    public class When_subscriber_has_subscribed
    {
        Establish context = () =>
        {
            var id = new SubscriptionId("id", "elders");
            projection = new SubscriberTokensProjection();
            subscriptionToken = new SubscriptionToken("token");
            subscriberId = new SubscriberId("kv", "elders");
            subscriptionType = SubscriptionType.FireBase;
            @event = new Subscribed(id, subscriberId, subscriptionToken, subscriptionType);
        };

        Because of = () => projection.Handle(@event);

        It should_subscribe_for_the_event = () => ((IProjectionDefinition)projection).GetProjectionIds(@event).ShouldContain(subscriberId);
        It should_handle_the_event = () => typeof(IEventHandler<Subscribed>).IsAssignableFrom(projection.GetType()).ShouldBeTrue();

        It should_have_correct_id = () => projection.State.SubscriberId.ShouldEqual(subscriberId);
        It should_have_correct_subscription_token_type_pair = () => projection.State.TokenTypePairs.ShouldNotBeNull();
        It should_have_correct_subscription_token = () => projection.State.TokenTypePairs.Single().SubscriptionToken.ShouldEqual(subscriptionToken);
        It should_have_correct_subscription_type = () => projection.State.TokenTypePairs.Single().SubscriptionType.ShouldEqual(subscriptionType);

        static SubscriberTokensProjection projection;
        static SubscriberId subscriberId;
        static SubscriptionToken subscriptionToken;
        static SubscriptionType subscriptionType;
        static Subscribed @event;
    }
}
