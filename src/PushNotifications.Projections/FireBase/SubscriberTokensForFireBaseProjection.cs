using System.Runtime.Serialization;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts;
using PushNotifications.Contracts.FireBaseSubscriptions.Events;

namespace PushNotifications.Projections.FireBase
{
    [DataContract(Name = "cab5caa4-192b-405c-a96c-438fe5a3ff70")]
    public class SubscriberTokensForFireBaseProjection : ProjectionDefinition<SubscriberTokens, SubscriberId>, IProjection,
        IEventHandler<SubscriberSubscribedForFireBase>,
        IEventHandler<SubscriberUnSubscribedFromFireBase>
    {
        public SubscriberTokensForFireBaseProjection()
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
