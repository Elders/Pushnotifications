using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elders.Cronus;
using Elders.Cronus.Projections;
using Elders.Cronus.Userfull;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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
                var result = await subscriptionManager.TrySubscribeToTopicAsync(@event.SubscriptionToken, topic);
                if (result is string errorMessage && (errorMessage.Contains("invalid-argument") || errorMessage.Contains("registration-token-not-registered")))
                {
                    _logger.LogInformation($"The token is invalid, the user will not be subscribed for this topic and the token will be removed. Subscriber: {@event.SubscriberId}, Topic: {topic}");

                    var deviceSubscriptionId = DeviceSubscriptionId.New(@event.Id.NID, @event.SubscriptionToken.Token);
                    UnSubscribe unSubscribe = new UnSubscribe(deviceSubscriptionId, @event.SubscriberId, @event.SubscriptionToken);
                    _publisher.Publish(unSubscribe);
                }
                else if (result is string)
                {
                    _logger.LogError($"Failed to subscribe for topic, look for the error for more info. Topic: {topic}, Subscriber: {@event.SubscriberId}");
                }
                else
                {
                    _logger.LogError($"Failed to subscribe from topic. Topic: {topic}, Subscriber: {@event.SubscriberId}");
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
                var result = await subscriptionManager.TryUnsubscribeFromTopicAsync(@event.SubscriptionToken, topic);

                if (result is string errorMessage && (errorMessage.Contains("invalid-argument") || errorMessage.Contains("registration-token-not-registered")))
                {
                    _logger.LogInformation($"The token is invalid and will be removed. Subscriber: {@event.SubscriberId}, Topic: {topic}");

                    var deviceSubscriptionId = DeviceSubscriptionId.New(@event.Id.NID, @event.SubscriptionToken.Token);
                    UnSubscribe unSubscribe = new UnSubscribe(deviceSubscriptionId, @event.SubscriberId, @event.SubscriptionToken);
                    _publisher.Publish(unSubscribe);
                }
                else if (result is string)
                {
                    _logger.LogError($"Failed to unsubscribe from topic, look for the error for more info. Topic: {topic}, Subscriber: {@event.SubscriberId}");
                }
                else
                {
                    _logger.LogError($"Failed to unsubscribe from topic. Topic: {@topic}, Subscriber: {@event.SubscriberId}");
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
                var result = await subscriptionManager.TrySubscribeToTopicAsync(token, @event.Id.Topic);

                if (result is string errorMessage && (errorMessage.Contains("invalid-argument") || errorMessage.Contains("registration-token-not-registered")))
                {
                    _logger.LogInformation($"The token is invalid, the user will not be subscribed for this topic and the token will be removed. Subscriber: {@event.Id.SubscriberId}, Topic: {@event.Id.Topic}");

                    var deviceSubscriptionId = DeviceSubscriptionId.New(@event.Id.NID, token.Token);
                    UnSubscribe unSubscribe = new UnSubscribe(deviceSubscriptionId, @event.Id.SubscriberId, token);
                    _publisher.Publish(unSubscribe);
                }
                else if (result is string)
                {
                    _logger.LogError($"Failed to subscribe for topic, look for the error for more info. Topic: {@event.Id.Topic}, Subscriber: {@event.Id.SubscriberId}");
                }
                else
                {
                    _logger.LogError($"Failed to subscribe from topic. Topic: {@event.Id.Topic}, Subscriber: {@event.Id.SubscriberId}");
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
                var result = await subscriptionManager.TryUnsubscribeFromTopicAsync(token, @event.Id.Topic);

                if (result is string errorMessage && (errorMessage.Contains("invalid-argument") || errorMessage.Contains("registration-token-not-registered")))
                {
                    _logger.LogInformation($"The token is invalid and will be removed. Subscriber: {@event.Id.SubscriberId}, Topic: {@event.Id.Topic}");

                    var deviceSubscriptionId = DeviceSubscriptionId.New(@event.Id.NID, token.Token);
                    UnSubscribe unSubscribe = new UnSubscribe(deviceSubscriptionId, @event.Id.SubscriberId, token);
                    _publisher.Publish(unSubscribe);
                }
                else if (result is string)
                {
                    _logger.LogError($"Failed to unsubscribe from topic, look for the error for more info. Topic: {@event.Id.Topic}, Subscriber: {@event.Id.SubscriberId}");
                }
                else
                {
                    _logger.LogError($"Failed to unsubscribe from topic. Topic: {@event.Id.Topic}, Subscriber: {@event.Id.SubscriberId}");
                }
            }
        }
    }
}
