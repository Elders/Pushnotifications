using Elders.Cronus.DomainModeling;
using Projections;
using Projections.Collections;
using PushNotifications.Contracts.Subscriptions.Events;
using System.Runtime.Serialization;

namespace PushNotifications.Ports.APNS
{
    public class APNSTokenWriteProjection : IProjectionHandler,
        IEventHandler<UserSubscribedForAPNS>,
        IEventHandler<UserUnSubscribedFromAPNS>
    {
        public IProjectionRepository Projections { get; set; }

        public void Handle(UserSubscribedForAPNS @event)
        {
            var projection = Projections.LoadCollectionItem<APNSTokenProjection>(@event.UserId, @event.Token);
            if (ReferenceEquals(projection, null) == true)
                projection = new APNSTokenProjection();

            projection.Handle(@event);
            Projections.SaveAsCollection(projection);
        }

        public void Handle(UserUnSubscribedFromAPNS @event)
        {
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
}
