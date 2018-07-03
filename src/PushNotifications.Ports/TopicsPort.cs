using System;
using Elders.Cronus;
using Elders.Cronus.Projections;
using Multitenancy.Delivery;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Ports.Logging;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Ports
{
    public class TopicsPort : IPort,
        IEventHandler<Subscribed>,
        IEventHandler<SubscribedToTopic>,
        IEventHandler<UnSubscribed>,
        IEventHandler<UnsubscribedFromTopic>
    {
        static ILog log = LogProvider.GetLogger(typeof(TopicsPort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IProjectionLoader Projections { get; set; }

        public IDeliveryProvisioner DeliveryProvisioner { get; set; }

        public void Handle(Subscribed @event) //In progress
        {
            var currentUser = @event.SubscriberId;
            var device = @event.SubscriptionToken.Token;

            var projectionReponse = Projections.Get<TopicsPerSubscriberProjection>(@event.SubscriberId);

            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No topics were found for subscriber {@event.SubscriberId}");
                return;
            }

            foreach (var topic in projectionReponse.Projection.State.Topics)
            {
                // Firebase/Pushy Call to subscribe current device for all topics that user is associated with
            }
        }

        public void Handle(UnSubscribed @event)
        {
            var currentUser = @event.SubscriberId;
            var device = @event.SubscriptionToken.Token;

            var projectionReponse = Projections.Get<TopicsPerSubscriberProjection>(@event.SubscriberId);

            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No topics were found for subscriber {@event.SubscriberId}");
                return;
            }

            foreach (var topic in projectionReponse.Projection.State.Topics)
            {
                // Firebase/Pushy Call to unsubscribe current device for all topics that user is associated with
            }
        }

        public void Handle(SubscribedToTopic @event)
        {
            if (ReferenceEquals(null, Projections)) throw new ArgumentNullException(nameof(Projections));
            if (ReferenceEquals(null, DeliveryProvisioner)) throw new ArgumentNullException(nameof(DeliveryProvisioner));

            var projectionReponse = Projections.Get<SubscriberTokensProjection>(@event.SubscriberId);
            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No tokens were found for subscriber {@event.SubscriberId}");
                return;
            }

            foreach (var token in projectionReponse.Projection.State.Tokens)
            {
                // Firebase/Pushy Call to subscribe all the devices to the current topic
            }
        }


        public void Handle(UnsubscribedFromTopic @event)
        {
            if (ReferenceEquals(null, Projections)) throw new ArgumentNullException(nameof(Projections));
            if (ReferenceEquals(null, DeliveryProvisioner)) throw new ArgumentNullException(nameof(DeliveryProvisioner));

            var projectionReponse = Projections.Get<SubscriberTokensProjection>(@event.SubscriberId);
            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No tokens were found for subscriber {@event.SubscriberId}");
                return;
            }

            foreach (var token in projectionReponse.Projection.State.Tokens)
            {
                // Firebase/Pushy Call to unsubscribe all the devices from the current topic
            }
        }
    }
}
