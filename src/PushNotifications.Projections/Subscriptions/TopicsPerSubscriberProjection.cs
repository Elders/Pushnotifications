using System.Runtime.Serialization;
using Elders.Cronus;
using Elders.Cronus.Projections;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Projections.Subscriptions
{
    [DataContract(Name = "fc35e537-f06b-406b-b6c3-94725853a278")]
    public class TopicsPerSubscriberProjection : ProjectionDefinition<SubscriberTopics, SubscriberId>, IProjection,
       IEventHandler<SubscribedToTopic>,
       IEventHandler<UnsubscribedFromTopic>
    {
        public TopicsPerSubscriberProjection()
        {
            Subscribe<SubscribedToTopic>(x => x.Id.SubscriberId);
            Subscribe<UnsubscribedFromTopic>(x => x.Id.SubscriberId);
        }

        public void Handle(SubscribedToTopic @event)
        {
            State.SubscriberId = @event.Id.SubscriberId;
            State.Topics.Add(@event.Id.Topic);
        }

        public void Handle(UnsubscribedFromTopic @event)
        {
            State.SubscriberId = @event.Id.SubscriberId;
            State.Topics.Remove(@event.Id.Topic);
        }
    }
}
