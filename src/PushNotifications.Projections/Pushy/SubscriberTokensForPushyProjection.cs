using System.Runtime.Serialization;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushySubscriptions.Events;

namespace PushNotifications.Projections.Pushy
{
    [DataContract(Name = "3e68e96c-c408-4cf1-91aa-3e3e29528439")]
    public class SubscriberTokensForPushyProjection : ProjectionDefinition<SubscriberTokens, SubscriberId>, IProjection,
        IEventHandler<SubscriberSubscribedForPushy>,
        IEventHandler<SubscriberUnSubscribedFromPushy>
    {
        public SubscriberTokensForPushyProjection()
        {
            Subscribe<SubscriberSubscribedForPushy>(x => x.SubscriberId);
            Subscribe<SubscriberUnSubscribedFromPushy>(x => x.SubscriberId);
        }

        public void Handle(SubscriberSubscribedForPushy @event)
        {
            State.SubscriberId = @event.SubscriberId;
            State.Tokens.Add(@event.Token);
        }

        public void Handle(SubscriberUnSubscribedFromPushy @event)
        {
            State.SubscriberId = @event.SubscriberId;
            State.Tokens.Remove(@event.Token);
        }
    }
}
