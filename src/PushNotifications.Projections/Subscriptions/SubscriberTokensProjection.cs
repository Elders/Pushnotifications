using System.Runtime.Serialization;
using System.Threading.Tasks;
using Elders.Cronus;
using Elders.Cronus.Projections;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Projections.Subscriptions
{
    [DataContract(Name = "d42d1bc2-6e83-471b-8c73-6ce2bace4f15")]
    public class SubscriberTokensProjection : ProjectionDefinition<SubscriberTokens, DeviceSubscriberId>, IProjection,
        IEventHandler<Subscribed>,
        IEventHandler<UnSubscribed>
    {
        public SubscriberTokensProjection()
        {
            Subscribe<Subscribed>(x => x.SubscriberId);
            Subscribe<UnSubscribed>(x => x.SubscriberId);
        }

        public Task HandleAsync(Subscribed @event)
        {
            State.SubscriberId = @event.SubscriberId;
            State.Tokens.Add(@event.SubscriptionToken);

            return Task.CompletedTask;
        }

        public Task HandleAsync(UnSubscribed @event)
        {
            State.SubscriberId = @event.SubscriberId;
            State.Tokens.Remove(@event.SubscriptionToken);

            return Task.CompletedTask;
        }
    }
}
