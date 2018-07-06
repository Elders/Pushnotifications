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
        [AuthorizeClaim(AuthorizeClaimType.Tenant, AuthorizeClaimType.TenantClient)]
        public string Tenant { get; set; }

        /// <summary>
        /// URN of the subscriber. This must be string tenant urn
        /// </summary>
        [ClaimsIdentity(AuthorizeClaimType.Subject, ClaimTypes.NameIdentifier)]
        public StringTenantUrn SubscriberUrn { get; set; }

        /// <summary>
        /// Registration token
        /// </summary>
        [Required]
        public string Token { get; set; }

        [Required]
        public string Topic { get; set; }

        public SubscribeToTopic AsSubscribeToTopicCommand()
        {
            var topic = new Topic(Topic);
            var subscriptionToken = new SubscriptionToken(Token, SubscriptionType.Pushy);
            var subscriptionId = new TopicSubscriptionId(subscriptionToken, Tenant);
            var subscriberId = new SubscriberId(SubscriberUrn.Id, SubscriberUrn.Tenant);
            return new SubscribeToTopic(subscriptionId, subscriberId, topic, subscriptionToken.SubscriptionType);
        }

        public UnsubscribeFromTopic AsUnsubscribeFromTopicCommand()
        {
            var topic = new Topic(Topic);
            var subscriptionToken = new SubscriptionToken(Token, SubscriptionType.Pushy);
            var subscriptionId = new TopicSubscriptionId(subscriptionToken, Tenant);
            var subscriberId = new SubscriberId(SubscriberUrn.Id, SubscriberUrn.Tenant);
            return new UnsubscribeFromTopic(subscriptionId, subscriberId, topic, subscriptionToken.SubscriptionType);
        }
    }
}
