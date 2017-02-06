using Elders.Cronus.DomainModeling;
using Projections;
using Projections.Collections;
using PushNotifications.Contracts.Subscriptions.Events;
using System.Runtime.Serialization;

namespace PushNotifications.Ports.Pushy
{
    public class PushyTokenWriteProjection : IProjectionHandler,
        IEventHandler<UserSubscribedForPushy>,
        IEventHandler<UserUnSubscribedFromPushy>
    {
        public IProjectionRepository Projections { get; set; }

        public void Handle(UserSubscribedForPushy @event)
        {
            var projection = Projections.LoadCollectionItem<PushyTokenProjection>(@event.UserId, @event.Token);
            if (ReferenceEquals(projection, null) == true)
                projection = new PushyTokenProjection();

            projection.Handle(@event);
            Projections.SaveAsCollection(projection);
        }

        public void Handle(UserUnSubscribedFromPushy @event)
        {
            Projections.DeleteCollectionItem<PushyTokenProjection>(@event.Token, @event.UserId);
        }
    }

    public class PushyTokenProjection : ProjectionCollectionDef<PushyTokenProjectionState>,
        IEventHandler<UserSubscribedForPushy>
    {
        public void Handle(UserSubscribedForPushy @event)
        {
            State.Id = @event.UserId;
            State.CollectionId = @event.Token;
            State.UserId = @event.UserId;
            State.Token = @event.Token;
        }
    }

    [DataContract(Name = "55d943c7-ea33-4546-ba73-2d7cfa52fd49")]
    public class PushyTokenProjectionState : ProjectionCollectionState<string, string>
    {
        PushyTokenProjectionState() { }

        [DataMember(Order = 1)]
        public string UserId { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }
    }
}
