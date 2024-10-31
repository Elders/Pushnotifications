using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elders.Cronus;
using Elders.Cronus.Projections;
using Microsoft.Extensions.Logging;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Ports
{
    public class TopicsSubscriptionsPort : IPort,
        IEventHandler<Subscribed>,
        IEventHandler<SubscribedToTopic>,
        IEventHandler<UnSubscribed>,
        IEventHandler<UnsubscribedFromTopic>
    {
        private readonly IPublisher<ICommand> _publisher;
        private readonly IProjectionReader _projections;
        private readonly ILogger<TopicsSubscriptionsPort> _logger;

        Dictionary<string, ITopicSubscriptionManager> deliveries;

        public TopicsSubscriptionsPort(IPublisher<ICommand> commandPublisher, IProjectionReader projections, ILogger<TopicsSubscriptionsPort> logger, IEnumerable<ITopicSubscriptionManager> deliveries)
        {
            _publisher = commandPublisher;
            _projections = projections;
            _logger = logger;
            this.deliveries = deliveries.ToDictionary(key => key.Platform.ToString());
        }

        public async Task HandleAsync(Subscribed @event)
        {
            SubscriptionType subscriptionType = @event.SubscriptionToken.SubscriptionType;
            ReadResult<TopicsPerSubscriberProjection> projectionReponse = await _projections.GetAsync<TopicsPerSubscriberProjection>(@event.SubscriberId);

            if (projectionReponse.IsSuccess == false)
            {
                _logger.Debug(() => $"No topics were found for subscriber {@event.SubscriberId}");
                return;
            }

            ITopicSubscriptionManager subscriptionManager = deliveries[subscriptionType];

            //TODO: This can be improved by sending all topics at asynchronously
            bool isSuccessful = true;
            foreach (Topic topic in projectionReponse.Data.State.Topics)
            {
                isSuccessful &= await subscriptionManager.SubscribeToTopicAsync(@event.SubscriptionToken, topic);
            }

            if (isSuccessful == false)
                _logger.LogError($"Failed to subscribe to topics for subscriber {@event.SubscriberId}");
        }

        public async Task HandleAsync(UnSubscribed @event)
        {
            SubscriptionType subscriptionType = @event.SubscriptionToken.SubscriptionType;
            ReadResult<TopicsPerSubscriberProjection> projectionReponse = await _projections.GetAsync<TopicsPerSubscriberProjection>(@event.SubscriberId);

            if (projectionReponse.IsSuccess == false)
            {
                _logger.Debug(() => $"No topics were found for subscriber {@event.SubscriberId},{@event.SubscriptionToken}");
                return;
            }

            ITopicSubscriptionManager subscriptionManager = deliveries[subscriptionType];


            //TODO: This can be improved by sending all topics at asynchronously
            bool isSuccessful = true;
            foreach (Topic topic in projectionReponse.Data.State.Topics)
            {
                isSuccessful &= await subscriptionManager.UnsubscribeFromTopicAsync(@event.SubscriptionToken, topic);
            }

            if (isSuccessful == false)
                _logger.LogError($"Failed to unsubscribe from topics for subscriber {@event.SubscriberId},{@event.SubscriptionToken}");
        }

        public async Task HandleAsync(SubscribedToTopic @event)
        {
            ReadResult<SubscriberTokensProjection> projectionReponse = await _projections.GetAsync<SubscriberTokensProjection>(@event.Id.SubscriberId);
            if (projectionReponse.IsSuccess == false)
            {
                _logger.Debug(() => $"No tokens were found for subscriber {@event.Id.SubscriberId}");
                return;
            }

            bool isSuccessful = true;
            foreach (SubscriptionToken token in projectionReponse.Data.State.Tokens)
            {
                ITopicSubscriptionManager subscriptionManager = deliveries[token.SubscriptionType];
                isSuccessful &= await subscriptionManager.SubscribeToTopicAsync(token, @event.Id.Topic);
            }

            if (isSuccessful == false)
                _logger.LogError($"Failed to subscribe to topic {@event.Id.Topic} for subscriber {@event.Id.SubscriberId}");
        }

        public async Task HandleAsync(UnsubscribedFromTopic @event)
        {
            ReadResult<SubscriberTokensProjection> projectionReponse = await _projections.GetAsync<SubscriberTokensProjection>(@event.Id.SubscriberId);
            if (projectionReponse.IsSuccess == false)
            {
                _logger.Debug(() => $"No tokens were found for subscriber {@event.Id.SubscriberId}");
                return;
            }

            bool isSuccessful = true;
            foreach (SubscriptionToken token in projectionReponse.Data.State.Tokens)
            {
                ITopicSubscriptionManager subscriptionManager = deliveries[token.SubscriptionType];
                isSuccessful &= await subscriptionManager.UnsubscribeFromTopicAsync(token, @event.Id.Topic);
            }

            if (isSuccessful == false)
                _logger.LogError($"Failed to unsubscribe from topic {@event.Id.Topic} for subscriber {@event.Id.SubscriberId}");
        }
    }
}
