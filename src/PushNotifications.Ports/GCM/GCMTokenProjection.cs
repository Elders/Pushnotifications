using Elders.Cronus.DomainModeling;
using Projections;
using Projections.Collections;
using PushNotifications.Contracts.Subscriptions.Events;
using System.Runtime.Serialization;

namespace PushNotifications.Ports.GCM
{
    public class GCMTokenWriteProjection : IProjectionHandler,
        IEventHandler<UserSubscribedForGCM>,
        IEventHandler<UserUnSubscribedFromGCM>
    {
        public IProjectionRepository Projections { get; set; }

        public void Handle(UserSubscribedForGCM @event)
        {
            var projection = Projections.LoadCollectionItem<GCMTokenProjection>(@event.UserId, @event.Token);
            if (ReferenceEquals(projection, null) == true)
                projection = new GCMTokenProjection();

            projection.Handle(@event);
            Projections.SaveAsCollection(projection);
        }

        public void Handle(UserUnSubscribedFromGCM @event)
        {
            Projections.DeleteCollectionItem<GCMTokenProjection>(@event.Token, @event.UserId);
        }
    }

    public class GCMTokenProjection : ProjectionCollectionDef<GCMTokenProjectionState>,
        IEventHandler<UserSubscribedForGCM>
    {
        public void Handle(UserSubscribedForGCM @event)
        {
            State.Id = @event.UserId;
            State.CollectionId = @event.Token;
            State.UserId = @event.UserId;
            State.Token = @event.Token;
        }
    }

    [DataContract(Name = "cea67031-945d-43ce-baee-b02b2e86afbf")]
    public class GCMTokenProjectionState : ProjectionCollectionState<string, string>
    {
        GCMTokenProjectionState() { }

        [DataMember(Order = 1)]
        public string UserId { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }
    }
}
