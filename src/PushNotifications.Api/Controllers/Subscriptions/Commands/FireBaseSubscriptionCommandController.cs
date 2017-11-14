﻿using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using PushNotifications.Contracts.FireBaseSubscriptions.Commands;
using PushNotifications.Contracts.FireBaseSubscriptions;
using PushNotifications.Contracts;
using PushNotifications.Api.Attributes;
using System.Security.Claims;
using Discovery.Contracts;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [RoutePrefix("Subscriptions/FireBaseSubscription")]
    public class FireBaseSubscriptionCommandController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        /// <summary>
        /// Subscribes for push notifications with FireBase token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Subscribe"), Discoverable("FireBaseSubscriptionSubscribe", "v1")]
        public IHttpActionResult SubscribeToFireBase(FireBaseSubscribeModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);
            if (Urn.IsUrn(model.SubscriberId) == false) return this.NotAcceptable($"{nameof(model.SubscriberId)} must be URN.");

            var command = model.AsSubscribeCommand();
            if (command.IsValid())
            {
                result = Publisher.Publish(command)
                    ? new ResponseResult<ResponseResult>(new ResponseResult())
                    : new ResponseResult(Constants.CommandPublishFailed);
            }
            return result.IsSuccess
                ? this.Accepted(result)
                : this.NotAcceptable(result);
        }

        /// <summary>
        /// UnSubscribes from push notifications with FireBase token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UnSubscribe"), Discoverable("FireBaseSubscriptionUnSubscribe", "v1")]
        public IHttpActionResult UnSubscribeFromFireBase(FireBaseSubscribeModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);
            if (Urn.IsUrn(model.SubscriberId) == false) return this.NotAcceptable($"{nameof(model.SubscriberId)} must be URN.");

            var command = model.AsUnSubscribeCommand();
            if (command.IsValid())
            {
                result = Publisher.Publish(command)
                    ? new ResponseResult<ResponseResult>(new ResponseResult())
                    : new ResponseResult(Constants.CommandPublishFailed);
            }
            return result.IsSuccess
                ? this.Accepted(result)
                : this.NotAcceptable(result);
        }
    }

    public class FireBaseSubscribeModel
    {
        [AuthorizeClaim(AuthorizeClaimType.Tenant, AuthorizeClaimType.TenantClient)]
        public string Tenant { get; set; }

        [ClaimsIdentity(AuthorizeClaimType.Subject, ClaimTypes.NameIdentifier)]
        public string SubscriberId { get; set; }

        [Required]
        public SubscriptionToken Token { get; set; }

        public SubscribeUserForFireBase AsSubscribeCommand()
        {
            var urn = StringTenantUrn.Parse(SubscriberId);
            var subscriptionId = new FireBaseSubscriptionId(Token, Tenant);
            var subscriberId = new SubscriberId(urn.Id, urn.Tenant);
            return new SubscribeUserForFireBase(subscriptionId, subscriberId, Token);
        }

        public UnSubscribeUserFromFireBase AsUnSubscribeCommand()
        {
            var urn = StringTenantUrn.Parse(SubscriberId);
            var subscriptionId = new FireBaseSubscriptionId(Token, Tenant);
            var subscriberId = new SubscriberId(urn.Id, urn.Tenant);
            return new UnSubscribeUserFromFireBase(subscriptionId, subscriberId, Token);
        }
    }
}