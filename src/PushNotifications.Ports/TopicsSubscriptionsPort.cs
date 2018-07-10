using System;
using Elders.Cronus;
using Elders.Cronus.Projections;
using Multitenancy.Delivery;
using PushNotifications.Contracts.Subscriptions.Events;
using PushNotifications.Ports.Logging;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Ports
{
    public class TopicsSubscriptionsPort : IPort,
        IEventHandler<Subscribed>,
        IEventHandler<SubscribedToTopic>,
        IEventHandler<UnSubscribed>,
        IEventHandler<UnsubscribedFromTopic>
    {
        static ILog log = LogProvider.GetLogger(typeof(TopicsSubscriptionsPort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IProjectionLoader Projections { get; set; }

        public IDeliveryProvisioner DeliveryProvisioner { get; set; }

        public ITopicSubscriptionProvisioner TopicSubscriptionProvider { get; set; }

        public void Handle(Subscribed @event)
        {
            var currentUser = @event.SubscriberId;
            var tenant = @event.SubscriberId.Tenant;
            var device = @event.SubscriptionToken.Token;
            var subscriptionType = @event.SubscriptionToken.SubscriptionType;

            var projectionReponse = Projections.Get<TopicsPerSubscriberProjection>(@event.SubscriberId);

            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No topics were found for subscriber {@event.SubscriberId}");
                return;
            }

            foreach (var topic in projectionReponse.Projection.State.Topics)
            {
                var subscriptionManager = TopicSubscriptionProvider.ResolveTopicSubscriptionManager(subscriptionType, tenant);
                subscriptionManager.SubscribeToTopic(@event.SubscriptionToken, topic);
            }
        }

        public void Handle(UnSubscribed @event)
        {
            var currentUser = @event.SubscriberId;
            var tenant = @event.SubscriberId.Tenant;
            var device = @event.SubscriptionToken.Token;
            var subscriptionType = @event.SubscriptionToken.SubscriptionType;

            var projectionReponse = Projections.Get<TopicsPerSubscriberProjection>(@event.SubscriberId);

            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No topics were found for subscriber {@event.SubscriberId}");
                return;
            }

            foreach (var topic in projectionReponse.Projection.State.Topics)
            {
                var subscriptionManager = TopicSubscriptionProvider.ResolveTopicSubscriptionManager(subscriptionType, tenant);
                subscriptionManager.UnsubscribeFromTopic(@event.SubscriptionToken, topic);
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
                var subscriptionManager = TopicSubscriptionProvider.ResolveTopicSubscriptionManager(token.SubscriptionType, @event.SubscriberId.Tenant);
                subscriptionManager.SubscribeToTopic(token, @event.Topic);
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
                var subscriptionManager = TopicSubscriptionProvider.ResolveTopicSubscriptionManager(@event.SubscriptionType, @event.SubscriberId.Tenant);
                subscriptionManager.UnsubscribeFromTopic(token, @event.Topic);
            }
        }
    }
}
