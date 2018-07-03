using System.Runtime.Serialization;
using Elders.Cronus;
using Elders.Cronus.Projections;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions.Events;

namespace PushNotifications.Projections.Subscriptions
{
    [DataContract(Name = "fc35e537-f06b-406b-b6c3-94725853a278")]
    public class TopicsPerSubscriberProjection : ProjectionDefinition<SubscriberTopics, SubscriberId>, IProjection,
       IEventHandler<SubscribedToTopic>,
       IEventHandler<UnsubscribedFromTopic>
    {
        public TopicsPerSubscriberProjection()
        {
            Subscribe<SubscribedToTopic>(x => x.SubscriberId);
            Subscribe<UnsubscribedFromTopic>(x => x.SubscriberId);
        }

        public void Handle(SubscribedToTopic @event)
        {
            State.SubscriberId = @event.SubscriberId;
            State.Topics.Add(@event.Topic);
        }

        public void Handle(UnsubscribedFromTopic @event)
        {
            State.SubscriberId = @event.SubscriberId;
            State.Topics.Remove(@event.Topic);
        }
    }
}
