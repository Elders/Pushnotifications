using System.Runtime.Serialization;
using System.Threading.Tasks;
using Elders.Cronus;
using Elders.Cronus.Projections;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Projections.Subscriptions
{
    [DataContract(Name = "fc35e537-f06b-406b-b6c3-94725853a278")]
    public class TopicsPerSubscriberProjection : ProjectionDefinition<SubscriberTopics, DeviceSubscriberId>, IProjection,
       IEventHandler<SubscribedToTopic>,
       IEventHandler<UnsubscribedFromTopic>
    {
        public TopicsPerSubscriberProjection()
        {
            Subscribe<SubscribedToTopic>(x => x.Id.SubscriberId);
            Subscribe<UnsubscribedFromTopic>(x => x.Id.SubscriberId);
        }

        public Task HandleAsync(SubscribedToTopic @event)
        {
            State.SubscriberId = @event.Id.SubscriberId;
            State.Topics.Add(@event.Id.Topic);

            return Task.CompletedTask;
        }

        public Task HandleAsync(UnsubscribedFromTopic @event)
        {
            State.SubscriberId = @event.Id.SubscriberId;
            State.Topics.Remove(@event.Id.Topic);

            return Task.CompletedTask;
        }
    }
}
