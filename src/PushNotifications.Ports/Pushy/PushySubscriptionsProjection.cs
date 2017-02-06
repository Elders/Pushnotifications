using Elders.Cronus.DomainModeling;
using Projections;
using Projections.Collections;
using PushNotifications.Contracts.Subscriptions.Events;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PushNotifications.Ports.Pushy
{
    public class PushySubscriptionsWriteProjection : IProjectionHandler,
        IEventHandler<UserSubscribedForPushy>,
        IEventHandler<UserUnSubscribedFromPushy>
    {
        public IProjectionRepository Projections { get; set; }

        public void Handle(UserSubscribedForPushy @event)
        {
            var projection = Projections.LoadCollectionItem<PushySubscriptionsProjection>(@event.UserId, @event.Token);
            if (ReferenceEquals(projection, null) == true)
                projection = new PushySubscriptionsProjection();

            projection.Handle(@event);
            Projections.SaveAsCollection(projection);

            var users = Projections.LoadCollectionItems<PushyTokenProjection>(@event.Token);
            foreach (var user in users)
            {
                Projections.DeleteCollectionItem<PushySubscriptionsProjection>(user.State.UserId, @event.Token);
                Projections.DeleteCollectionItem<PushyTokenProjection>(@event.Token, user.State.UserId);
            }
        }

        public void Handle(UserUnSubscribedFromPushy @event)
        {
            Projections.DeleteCollectionItem<PushySubscriptionsProjection>(@event.UserId, @event.Token);
        }
    }

    public class PushySubscriptionsProjection : ProjectionCollectionDef<PushySubscriptionsProjectionState>,
        IEventHandler<UserSubscribedForPushy>
    {
        public void Handle(UserSubscribedForPushy @event)
        {
            State.Id = @event.Token;
            State.CollectionId = @event.UserId;
            State.UserId = @event.UserId;
            State.Token = @event.Token;
            State.Badge = 0;
        }
    }

    [DataContract(Name = "165730b5-64f5-4ebe-a381-721fd220713f")]
    public class PushySubscriptionsProjectionState : ProjectionCollectionState<string, string>
    {
        PushySubscriptionsProjectionState() { }

        [DataMember(Order = 1)]
        public string UserId { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public int Badge { get; set; }

        public static IEqualityComparer<PushySubscriptionsProjectionState> Comparer { get { return new PushySubscriptionsComparer(); } }

        public class PushySubscriptionsComparer : IEqualityComparer<PushySubscriptionsProjectionState>
        {
            public bool Equals(PushySubscriptionsProjectionState x, PushySubscriptionsProjectionState y)
            {
                if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
                    return true;
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                    return false;
                return x.UserId == y.UserId && x.Token == y.Token;
            }

            public int GetHashCode(PushySubscriptionsProjectionState obj)
            {
                return (117 ^ obj.UserId.GetHashCode()) ^ obj.Token.GetHashCode();
            }
        }
    }
}
