using System.Collections.Generic;
using System.Runtime.Serialization;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts;
using PushNotifications.Contracts.FireBaseSubscriptions.Events;

namespace PushNotifications.Projections
{
    [DataContract(Name = "cab5caa4-192b-405c-a96c-438fe5a3ff70")]
    public class SubscriberTokensProjection : ProjectionDefinition<SubscriberTokens, SubscriberId>, IProjection,
        IEventHandler<SubscriberSubscribedForFireBase>,
        IEventHandler<SubscriberUnSubscribedFromFireBase>
    {
        public SubscriberTokensProjection()
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

    [DataContract(Name = "f648e05a-b5d9-4947-ade6-789b7ffb3601")]
    public class SubscriberTokens
    {
        public SubscriberTokens()
        {
            Tokens = new HashSet<SubscriptionToken>();
        }

        [DataMember(Order = 1)]
        public SubscriberId SubscriberId { get; set; }

        [DataMember(Order = 2)]
        public HashSet<SubscriptionToken> Tokens { get; private set; }
    }
}
