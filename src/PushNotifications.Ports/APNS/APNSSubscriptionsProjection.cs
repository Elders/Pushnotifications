using Elders.Cronus.DomainModeling;
using Projections;
using Projections.Collections;
using PushNotifications.Contracts.Subscriptions.Events;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace PushNotifications.Ports.APNS
{
    public class APNSSubscriptionsWriteProjection : IProjectionHandler,
        IEventHandler<UserSubscribedForAPNS>,
        IEventHandler<UserUnSubscribedFromAPNS>
    {
        public IProjectionRepository Projections { get; set; }

        public void Handle(UserSubscribedForAPNS @event)
        {
            var users = Projections.LoadCollectionItems<APNSTokenProjection>(@event.Token).Where(x => ReferenceEquals(x, null) == false).Select(x => x.State);
            foreach (var user in users)
            {
                Projections.DeleteCollectionItem<APNSSubscriptionsProjection>(user.UserId, @event.Token);
                Projections.DeleteCollectionItem<APNSTokenProjection>(@event.Token, user.UserId);
            }

            var tokenProjection = new APNSTokenProjection();
            tokenProjection.Handle(@event);
            Projections.SaveAsCollection(tokenProjection);

            var projection = new APNSSubscriptionsProjection();
            projection.Handle(@event);
            Projections.SaveAsCollection(projection);
        }

        public void Handle(UserUnSubscribedFromAPNS @event)
        {
            Projections.DeleteCollectionItem<APNSSubscriptionsProjection>(@event.UserId, @event.Token);
            Projections.DeleteCollectionItem<APNSTokenProjection>(@event.Token, @event.UserId);
        }
    }

    public class APNSTokenProjection : ProjectionCollectionDef<APNSTokenProjectionState>,
        IEventHandler<UserSubscribedForAPNS>
    {
        public void Handle(UserSubscribedForAPNS @event)
        {
            State.Id = @event.UserId;
            State.CollectionId = @event.Token;
            State.UserId = @event.UserId;
            State.Token = @event.Token;
        }
    }

    [DataContract(Name = "aad15819-07fd-4c90-9d54-e86797dbb435")]
    public class APNSTokenProjectionState : ProjectionCollectionState<string, string>
    {
        APNSTokenProjectionState() { }

        [DataMember(Order = 1)]
        public string UserId { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }
    }

    public class APNSSubscriptionsProjection : ProjectionCollectionDef<APNSSubscriptionsProjectionState>,
        IEventHandler<UserSubscribedForAPNS>
    {
        public void Handle(UserSubscribedForAPNS @event)
        {
            State.Id = @event.Token;
            State.CollectionId = @event.UserId;
            State.UserId = @event.UserId;
            State.Token = @event.Token;
            State.Badge = 0;
        }
    }

    [DataContract(Name = "17bad271-0b44-44cf-a613-bc1e3702cd06")]
    public class APNSSubscriptionsProjectionState : ProjectionCollectionState<string, string>
    {
        APNSSubscriptionsProjectionState() { }

        [DataMember(Order = 1)]
        public string UserId { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public int Badge { get; set; }

        public static IEqualityComparer<APNSSubscriptionsProjectionState> Comparer { get { return new APNSSubscriptionsComparer(); } }

        public class APNSSubscriptionsComparer : IEqualityComparer<APNSSubscriptionsProjectionState>
        {
            public bool Equals(APNSSubscriptionsProjectionState x, APNSSubscriptionsProjectionState y)
            {
                if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
                    return true;
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                    return false;
                return x.UserId == y.UserId && x.Token == y.Token;
            }

            public int GetHashCode(APNSSubscriptionsProjectionState obj)
            {
                return (117 ^ obj.UserId.GetHashCode()) ^ obj.Token.GetHashCode();
            }
        }
    }
}
