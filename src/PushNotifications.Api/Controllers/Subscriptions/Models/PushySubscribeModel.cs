using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.ComponentModel.DataAnnotations;
using PushNotifications.Contracts;
using PushNotifications.Api.Attributes;
using System.Security.Claims;
using PushNotifications.Contracts.Subscriptions.Commands;
using PushNotifications.Contracts.Subscriptions;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    public class PushySubscribeModel
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
        public SubscriptionToken Token { get; set; }

        public Subscribe AsSubscribeCommand()
        {
            var subscriptionId = new SubscriptionId(Token, Tenant);
            var subscriberId = new SubscriberId(SubscriberUrn.Id, SubscriberUrn.Tenant);
            return new Subscribe(subscriptionId, subscriberId, Token, SubscriptionType.Pushy);
        }

        public UnSubscribe AsUnSubscribeCommand()
        {
            var subscriptionId = new SubscriptionId(Token, Tenant);
            var subscriberId = new SubscriberId(SubscriberUrn.Id, SubscriberUrn.Tenant);
            return new UnSubscribe(subscriptionId, subscriberId, Token, SubscriptionType.Pushy);
        }
    }
}
