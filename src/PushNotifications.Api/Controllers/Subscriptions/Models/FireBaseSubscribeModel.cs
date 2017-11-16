using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.ComponentModel.DataAnnotations;
using PushNotifications.Contracts.FireBaseSubscriptions.Commands;
using PushNotifications.Contracts.FireBaseSubscriptions;
using PushNotifications.Contracts;
using PushNotifications.Api.Attributes;
using System.Security.Claims;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    public class FireBaseSubscribeModel
    {
        [AuthorizeClaim(AuthorizeClaimType.Tenant, AuthorizeClaimType.TenantClient)]
        public string Tenant { get; set; }

        [ClaimsIdentity(AuthorizeClaimType.Subject, ClaimTypes.NameIdentifier)]
        public StringTenantUrn SubscriberUrn { get; set; }

        [Required]
        public SubscriptionToken Token { get; set; }

        public SubscribeUserForFireBase AsSubscribeCommand()
        {
            var subscriptionId = new FireBaseSubscriptionId(Token, Tenant);
            var subscriberId = new SubscriberId(SubscriberUrn.Id, SubscriberUrn.Tenant);
            return new SubscribeUserForFireBase(subscriptionId, subscriberId, Token);
        }

        public UnSubscribeUserFromFireBase AsUnSubscribeCommand()
        {
            var subscriptionId = new FireBaseSubscriptionId(Token, Tenant);
            var subscriberId = new SubscriberId(SubscriberUrn.Id, SubscriberUrn.Tenant);
            return new UnSubscribeUserFromFireBase(subscriptionId, subscriberId, Token);
        }
    }
}
