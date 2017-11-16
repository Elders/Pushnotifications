﻿using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.ComponentModel.DataAnnotations;
using PushNotifications.Contracts.PushySubscriptions.Commands;
using PushNotifications.Contracts.PushySubscriptions;
using PushNotifications.Contracts;
using PushNotifications.Api.Attributes;
using System.Security.Claims;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    public class PushySubscribeModel
    {
        [AuthorizeClaim(AuthorizeClaimType.Tenant, AuthorizeClaimType.TenantClient)]
        public string Tenant { get; set; }

        [ClaimsIdentity(AuthorizeClaimType.Subject, ClaimTypes.NameIdentifier)]
        public StringTenantUrn SubscriberUrn { get; set; }

        [Required]
        public SubscriptionToken Token { get; set; }

        public SubscribeUserForPushy AsSubscribeCommand()
        {
            var subscriptionId = new PushySubscriptionId(Token, Tenant);
            var subscriberId = new SubscriberId(SubscriberUrn.Id, SubscriberUrn.Tenant);
            return new SubscribeUserForPushy(subscriptionId, subscriberId, Token);
        }

        public UnSubscribeUserFromPushy AsUnSubscribeCommand()
        {
            var subscriptionId = new PushySubscriptionId(Token, Tenant);
            var subscriberId = new SubscriberId(SubscriberUrn.Id, SubscriberUrn.Tenant);
            return new UnSubscribeUserFromPushy(subscriptionId, subscriberId, Token);
        }
    }
}
