using Elders.Cronus.DomainModeling;
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
    public class FireBaseSubscriptionController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        [HttpPost, Route("Subscribe"), Discoverable("FireBaseSubscriptionSubscribe", "v1")]
        public IHttpActionResult Subscribe(FireBaseSubscriptionModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);

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

        [HttpPost, Route("UnSubscribe"), Discoverable("FireBaseSubscriptionUnSubscribe", "v1")]
        public IHttpActionResult UnSubscribe(FireBaseSubscriptionModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);

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

    public class FireBaseSubscriptionModel
    {
        [AuthorizeClaim(AuthorizeClaimType.Tenant, AuthorizeClaimType.TenantClient)]
        public string Tenant { get; set; }

        [ClaimsIdentity(AuthorizeClaimType.Subject, ClaimTypes.NameIdentifier)]
        public string UserId { get; set; }

        [Required]
        public SubscriptionToken Token { get; set; }

        public SubscribeUserForFireBase AsSubscribeCommand()
        {
            var urn = StringTenantUrn.Parse(UserId);
            var subscriptionId = new FireBaseSubscriptionId(Token, Tenant);
            var subscriberId = new SubscriberId(urn.Id, urn.Tenant);
            return new SubscribeUserForFireBase(subscriptionId, subscriberId, Token);
        }

        public UnSubscribeUserFromFireBase AsUnSubscribeCommand()
        {
            var urn = StringTenantUrn.Parse(UserId);
            var subscriptionId = new FireBaseSubscriptionId(Token, Tenant);
            var subscriberId = new SubscriberId(urn.Id, urn.Tenant);
            return new UnSubscribeUserFromFireBase(subscriptionId, subscriberId, Token);
        }
    }
}
