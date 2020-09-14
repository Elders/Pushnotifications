using Elders.Cronus;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Commands;
using System.ComponentModel.DataAnnotations;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    public class TopicSubscriptionModel
    {
        /// <summary>
        /// URN of the subscriber. This must be string tenant urn
        /// </summary>
        [Required, IsUrn]
        public string SubscriberId { get; set; }

        [Required]
        public string Topic { get; set; }

        public SubscribeToTopic AsSubscribeToTopicCommand()
        {
            var urn = AggregateUrn.Parse(SubscriberId, Urn.Uber);
            var id = new SubscriberId(urn);

            var topic = new Topic(Topic);

            var topicSubscriptionId = new TopicSubscriptionId(id, topic, urn.Tenant);
            return new SubscribeToTopic(topicSubscriptionId, id, topic);
        }

        public UnsubscribeFromTopic AsUnSubscribeFromTopicCommand()
        {
            var urn = AggregateUrn.Parse(SubscriberId, Urn.Uber);
            var id = new SubscriberId(urn);

            var topic = new Topic(Topic);
            var topicSubscriptionId = new TopicSubscriptionId(id, topic, urn.Tenant);
            return new UnsubscribeFromTopic(topicSubscriptionId, id, topic);
        }
    }
}
