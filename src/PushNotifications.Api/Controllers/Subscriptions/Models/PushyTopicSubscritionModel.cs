using Elders.Cronus;
using Elders.Web.Api;
using System.ComponentModel.DataAnnotations;
using PushNotifications.Contracts;
using PushNotifications.Api.Attributes;
using System.Security.Claims;
using PushNotifications.Contracts.Subscriptions.Commands;
using PushNotifications.Contracts.Subscriptions;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    public class PushySubscribeToTopicModel
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
            var subscriptionId = new TopicSubscriptionId(SubscriberId, topic, SubscriberId.Tenant);
            return new SubscribeToTopic(subscriptionId, SubscriberId, topic, SubscriptionType.Pushy);
        }

        public UnsubscribeFromTopic AsUnsubscribeFromTopicCommand()
        {
            var topic = new Topic(Topic);
            var subscriptionId = new TopicSubscriptionId(SubscriberId, topic, SubscriberId.Tenant);
            return new UnsubscribeFromTopic(subscriptionId, SubscriberId, topic, SubscriptionType.Pushy);
        }
    }
}
