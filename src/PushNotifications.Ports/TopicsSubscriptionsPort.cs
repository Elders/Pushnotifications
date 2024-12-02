using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elders.Cronus;
using Elders.Cronus.Projections;
using Microsoft.Extensions.Logging;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Commands;
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
            foreach (Topic topic in projectionReponse.Data.State.Topics)
            {
                var result = await subscriptionManager.SubscribeToTopicAsync(@event.SubscriptionToken, topic);
                if (result.IsSuccess == false)
                {
                    RemoveInvalidSubscribingToken(result.HasInvalidTokens, @event.Id, @event.SubscriberId, @event.SubscriptionToken, topic);
                }
            }
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
            foreach (Topic topic in projectionReponse.Data.State.Topics)
            {
                var result = await subscriptionManager.UnsubscribeFromTopicAsync(@event.SubscriptionToken, topic);
                if (result.IsSuccess == false)
                {
                    RemoveInvalidUnSubscribingToken(result.HasInvalidTokens, @event.Id, @event.SubscriberId, @event.SubscriptionToken, topic);
                }
            }
        }

        public async Task HandleAsync(SubscribedToTopic @event)
        {
            ReadResult<SubscriberTokensProjection> projectionReponse = await _projections.GetAsync<SubscriberTokensProjection>(@event.Id.SubscriberId);
            if (projectionReponse.IsSuccess == false)
            {
                _logger.Debug(() => $"No tokens were found for subscriber {@event.Id.SubscriberId}");
                return;
            }

            foreach (SubscriptionToken token in projectionReponse.Data.State.Tokens)
            {
                ITopicSubscriptionManager subscriptionManager = deliveries[token.SubscriptionType];
                var result = await subscriptionManager.SubscribeToTopicAsync(token, @event.Id.Topic);
                if (result.IsSuccess == false)
                {
                    var deviceSubscriptionId = DeviceSubscriptionId.New(@event.Id.NID, token.Token);
                    RemoveInvalidSubscribingToken(result.HasInvalidTokens, deviceSubscriptionId, @event.Id.SubscriberId, token, @event.Id.Topic);
                }
            }
        }

        public async Task HandleAsync(UnsubscribedFromTopic @event)
        {
            ReadResult<SubscriberTokensProjection> projectionReponse = await _projections.GetAsync<SubscriberTokensProjection>(@event.Id.SubscriberId);
            if (projectionReponse.IsSuccess == false)
            {
                _logger.Debug(() => $"No tokens were found for subscriber {@event.Id.SubscriberId}");
                return;
            }

            foreach (SubscriptionToken token in projectionReponse.Data.State.Tokens)
            {
                ITopicSubscriptionManager subscriptionManager = deliveries[token.SubscriptionType];
                var result = await subscriptionManager.UnsubscribeFromTopicAsync(token, @event.Id.Topic);
                if (result.IsSuccess == false)
                {
                    var deviceSubscriptionId = DeviceSubscriptionId.New(@event.Id.NID, token.Token);
                    RemoveInvalidUnSubscribingToken(result.HasInvalidTokens, deviceSubscriptionId, @event.Id.SubscriberId, token, @event.Id.Topic);
                }
            }
        }

        private void RemoveInvalidSubscribingToken(bool hasInvalidTokens, DeviceSubscriptionId id, DeviceSubscriberId subscriberId, SubscriptionToken subscriptionToken, Topic topic)
        {
            if (hasInvalidTokens)
            {
                _logger.LogInformation($"The token is invalid, the user will not be subscribed for this topic and the token will be removed. Subscriber: {subscriberId}, Topic: {topic}, Token:{subscriptionToken}");

                UnSubscribe unSubscribe = new UnSubscribe(id, subscriberId, subscriptionToken);
                _publisher.Publish(unSubscribe);
            }
            else
            {
                _logger.LogError($"Failed to subscribe for topic, look for the error for more info. Topic: {topic}, Subscriber: {subscriberId}");
            }
        }
        private void RemoveInvalidUnSubscribingToken(bool hasInvalidTokens, DeviceSubscriptionId id, DeviceSubscriberId subscriberId, SubscriptionToken subscriptionToken, Topic topic)
        {
            if (hasInvalidTokens)
            {
                _logger.LogInformation($"The token is invalid and will be removed. Subscriber: {subscriberId}, Topic: {topic}, Token:{subscriptionToken}");

                UnSubscribe unSubscribe = new UnSubscribe(id, subscriberId, subscriptionToken);
                _publisher.Publish(unSubscribe);
            }
            else
            {
                _logger.LogError($"Failed to unsubscribe from topic, look for the error for more info. Topic: {topic}, Subscriber: {subscriberId}");
            }
        }
    }
}
