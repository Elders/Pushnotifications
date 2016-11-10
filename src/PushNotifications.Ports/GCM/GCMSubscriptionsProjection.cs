using Elders.Cronus.DomainModeling;
using Projections;
using Projections.Collections;
using PushNotifications.Contracts.Subscriptions.Events;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PushNotifications.Ports.GCM
{
    public class GCMSubscriptionsWriteProjection : IProjectionHandler,
        IEventHandler<UserSubscribedForGCM>,
        IEventHandler<UserUnSubscribedFromGCM>
    {
        public IProjectionRepository Projections { get; set; }

        public void Handle(UserSubscribedForGCM @event)
        {
            var projection = Projections.LoadCollectionItem<GCMSubscriptionsProjection>(@event.UserId, @event.Token);
            if (ReferenceEquals(projection, null) == true)
                projection = new GCMSubscriptionsProjection();

            projection.Handle(@event);
            Projections.SaveAsCollection(projection);

            var users = Projections.LoadCollectionItems<GCMTokenProjection>(@event.Token);
            foreach (var user in users)
            {
                Projections.DeleteCollectionItem<GCMSubscriptionsProjection>(user.State.UserId, @event.Token);
                Projections.DeleteCollectionItem<GCMTokenProjection>(@event.Token, user.State.UserId);
            }
        }

        public void Handle(UserUnSubscribedFromGCM @event)
        {
            Projections.DeleteCollectionItem<GCMSubscriptionsProjection>(@event.UserId, @event.Token);
        }
    }

    public class GCMSubscriptionsProjection : ProjectionCollectionDef<GCMSubscriptionsProjectionState>,
        IEventHandler<UserSubscribedForGCM>
    {
        public void Handle(UserSubscribedForGCM @event)
        {
            State.Id = @event.Token;
            State.CollectionId = @event.UserId;
            State.UserId = @event.UserId;
            State.Token = @event.Token;
            State.Badge = 0;
        }
    }

    [DataContract(Name = "9a050d0d-21b4-403d-8086-40dc0efe61cc")]
    public class GCMSubscriptionsProjectionState : ProjectionCollectionState<string, string>
    {
        GCMSubscriptionsProjectionState() { }

        [DataMember(Order = 1)]
        public string UserId { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public int Badge { get; set; }

        public static IEqualityComparer<GCMSubscriptionsProjectionState> Comparer { get { return new GCMSubscriptionsComparer(); } }

        public class GCMSubscriptionsComparer : IEqualityComparer<GCMSubscriptionsProjectionState>
        {
            public bool Equals(GCMSubscriptionsProjectionState x, GCMSubscriptionsProjectionState y)
            {
                if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
                    return true;
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                    return false;
                return x.UserId == y.UserId && x.Token == y.Token;
            }

            public int GetHashCode(GCMSubscriptionsProjectionState obj)
            {
                return (117 ^ obj.UserId.GetHashCode()) ^ obj.Token.GetHashCode();
            }
        }
    }
}
