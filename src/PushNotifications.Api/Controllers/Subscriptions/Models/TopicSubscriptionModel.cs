using Elders.Web.Api;
using System.ComponentModel.DataAnnotations;
using PushNotifications.Contracts;
using PushNotifications.Api.Attributes;
using System.Security.Claims;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Commands;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    public class TopicSubscriptionModel
    {
        /// <summary>
        /// URN of the subscriber. This must be string tenant urn
        /// </summary>
        [ClaimsIdentity(AuthorizeClaimType.Subject, ClaimTypes.NameIdentifier)]
        public SubscriberId SubscriberId { get; set; }

        [Required]
        public Topic Topic { get; set; }

        public SubscribeToTopic AsSubscribeToTopicCommand()
        {
            var topic = new Topic(Topic);
            var sub = new SubscriberId(SubscriberId.Id, SubscriberId.Tenant);
            var topicSubscriptionId = new TopicSubscriptionId(SubscriberId, topic, SubscriberId.Tenant);
            return new SubscribeToTopic(topicSubscriptionId, SubscriberId, topic);
        }

        public UnsubscribeFromTopic AsUnSubscribeFromTopicCommand()
        {
            var topic = new Topic(Topic);
            var topicSubscriptionId = new TopicSubscriptionId(SubscriberId, topic, SubscriberId.Tenant);
            return new UnsubscribeFromTopic(topicSubscriptionId, SubscriberId, topic);
        }
    }
}
