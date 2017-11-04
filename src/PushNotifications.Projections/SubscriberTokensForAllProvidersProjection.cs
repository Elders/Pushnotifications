using System.Runtime.Serialization;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts;
using PushNotifications.Contracts.FireBaseSubscriptions.Events;

namespace PushNotifications.Projections
{
    [DataContract(Name = "6f475227-10e7-4f3e-821e-162a2cfcbdc1")]
    public class SubscriberTokensForAllProvidersProjection : ProjectionDefinition<SubscriberTokens, SubscriberId>, IProjection,
        IEventHandler<SubscriberSubscribedForFireBase>,
        IEventHandler<SubscriberUnSubscribedFromFireBase>
    {
        public SubscriberTokensForAllProvidersProjection()
        {
            Subscribe<SubscriberSubscribedForFireBase>(x => x.SubscriberId);
            Subscribe<SubscriberUnSubscribedFromFireBase>(x => x.SubscriberId);
        }

        public void Handle(SubscriberSubscribedForFireBase @event)
        {
            State.SubscriberId = @event.SubscriberId;
            State.Tokens.Add(@event.Token);
        }

        public void Handle(SubscriberUnSubscribedFromFireBase @event)
        {
            State.SubscriberId = @event.SubscriberId;
            State.Tokens.Remove(@event.Token);
        }
    }
}
