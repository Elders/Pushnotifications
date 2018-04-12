using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.ComponentModel.DataAnnotations;
using PushNotifications.Contracts;
using PushNotifications.Api.Attributes;
using System.Security.Claims;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Commands;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    public class FireBaseSubscribeModel
    {
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

        public Subscribe AsSubscribeCommand()
        {
            var subscriptionToken = new SubscriptionToken(Token, SubscriptionType.FireBase);
            var subscriptionId = new SubscriptionId(subscriptionToken, SubscriberUrn.Tenant);
            var subscriberId = new SubscriberId(SubscriberUrn.Id, SubscriberUrn.Tenant);
            return new Subscribe(subscriptionId, subscriberId, subscriptionToken);
        }

        public UnSubscribe AsUnSubscribeCommand()
        {
            var subscriptionToken = new SubscriptionToken(Token, SubscriptionType.FireBase);
            var subscriptionId = new SubscriptionId(subscriptionToken, SubscriberUrn.Tenant);
            var subscriberId = new SubscriberId(SubscriberUrn.Id, SubscriberUrn.Tenant);
            return new UnSubscribe(subscriptionId, subscriberId, subscriptionToken);
        }
    }
}
