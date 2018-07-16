using System;
using Elders.Cronus;
using Elders.Cronus.Projections;
using Multitenancy.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.Subscriptions;
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
            SubscriberId currentUser = @event.SubscriberId;
            string tenant = @event.SubscriberId.Tenant;
            string device = @event.SubscriptionToken.Token;
            SubscriptionType subscriptionType = @event.SubscriptionToken.SubscriptionType;

            var projectionReponse = Projections.Get<TopicsPerSubscriberProjection>(@event.SubscriberId);

            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No topics were found for subscriber {@event.SubscriberId}");
                return;
            }

            ITopicSubscriptionManager subscriptionManager = TopicSubscriptionProvider.ResolveTopicSubscriptionManager(subscriptionType, tenant);

            foreach (Topic topic in projectionReponse.Projection.State.Topics)
            {
                subscriptionManager.SubscribeToTopic(@event.SubscriptionToken, topic);
            }
        }

        public void Handle(UnSubscribed @event)
        {
            SubscriberId currentUser = @event.SubscriberId;
            string tenant = @event.SubscriberId.Tenant;
            string device = @event.SubscriptionToken.Token;
            SubscriptionType subscriptionType = @event.SubscriptionToken.SubscriptionType;

            var projectionReponse = Projections.Get<TopicsPerSubscriberProjection>(@event.SubscriberId);

            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No topics were found for subscriber {@event.SubscriberId}");
                return;
            }

            ITopicSubscriptionManager subscriptionManager = TopicSubscriptionProvider.ResolveTopicSubscriptionManager(subscriptionType, tenant);

            foreach (Topic topic in projectionReponse.Projection.State.Topics)
            {
                subscriptionManager.UnsubscribeFromTopic(@event.SubscriptionToken, topic);
            }
        }

        public void Handle(SubscribedToTopic @event)
        {
            if (ReferenceEquals(null, Projections)) throw new ArgumentNullException(nameof(Projections));
            if (ReferenceEquals(null, DeliveryProvisioner)) throw new ArgumentNullException(nameof(DeliveryProvisioner));

            var projectionReponse = Projections.Get<SubscriberTokensProjection>(@event.Id.SubscriberId);
            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No tokens were found for subscriber {@event.Id.SubscriberId}");
                return;
            }

            foreach (SubscriptionToken token in projectionReponse.Projection.State.Tokens)
            {
                ITopicSubscriptionManager subscriptionManager = TopicSubscriptionProvider.ResolveTopicSubscriptionManager(token.SubscriptionType, @event.Id.SubscriberId.Tenant);
                subscriptionManager.SubscribeToTopic(token, @event.Id.Topic);
            }
        }

        public void Handle(UnsubscribedFromTopic @event)
        {
            if (ReferenceEquals(null, Projections)) throw new ArgumentNullException(nameof(Projections));
            if (ReferenceEquals(null, DeliveryProvisioner)) throw new ArgumentNullException(nameof(DeliveryProvisioner));

            var projectionReponse = Projections.Get<SubscriberTokensProjection>(@event.Id.SubscriberId);
            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No tokens were found for subscriber {@event.Id.SubscriberId}");
                return;
            }

            foreach (SubscriptionToken token in projectionReponse.Projection.State.Tokens)
            {
                ITopicSubscriptionManager subscriptionManager = TopicSubscriptionProvider.ResolveTopicSubscriptionManager(token.SubscriptionType, @event.Id.SubscriberId.Tenant);
                subscriptionManager.UnsubscribeFromTopic(token, @event.Id.Topic);
            }
        }
    }
}
