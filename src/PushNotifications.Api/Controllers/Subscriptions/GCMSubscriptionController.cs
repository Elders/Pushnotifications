using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Commands;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using Thinktecture.IdentityModel.WebApi;
using Discovery;

namespace PushNotifications.Api.Controllers.Subscriptions
{
    [ScopeAuthorize("owner")]
    [RoutePrefix("Subscriptions/GCMSubscription")]
    public class GCMSubscriptionController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        [HttpPost, Route("Subscribe"), Discoverable("GCMSubscriptionSubscribe", "v1")]
        public IHttpActionResult Subscribe(GCMSubscriptionModel model)
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

        [HttpPost, Route("UnSubscribe"), Discoverable("GCMSubscriptionUnSubscribe", "v1")]
        public IHttpActionResult UnSubscribe(GCMSubscriptionModel model)
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

    public class GCMSubscriptionModel
    {
        [AuthorizeClaim(AuthorizeClaimType.Subject)]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        public SubscribeForGCM AsSubscribeCommand()
        {
            var id = new GCMSubscriptionId(Token);
            return new SubscribeForGCM(id, UserId, Token);
        }

        public UnSubscribeFromGCM AsUnSubscribeCommand()
        {
            var id = new GCMSubscriptionId(Token);
            return new UnSubscribeFromGCM(id, UserId, Token);
        }
    }
}
