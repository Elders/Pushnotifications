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
    [RoutePrefix("Subscriptions/PushySubscription")]
    public class PushySubscriptionController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        [HttpPost, Route("Subscribe"), Discoverable("PushySubscriptionSubscribe", "v1")]
        public IHttpActionResult Subscribe(PushySubscriptionModel model)
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

        [HttpPost, Route("UnSubscribe"), Discoverable("PushySubscriptionUnSubscribe", "v1")]
        public IHttpActionResult UnSubscribe(PushySubscriptionModel model)
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

    public class PushySubscriptionModel
    {
        [AuthorizeClaim(AuthorizeClaimType.Subject)]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        public SubscribeForPushy AsSubscribeCommand()
        {
            var id = new PushySubscriptionId(Token);
            return new SubscribeForPushy(id, UserId, Token);
        }

        public UnSubscribeFromPushy AsUnSubscribeCommand()
        {
            var id = new PushySubscriptionId(Token);
            return new UnSubscribeFromPushy(id, UserId, Token);
        }
    }
}
