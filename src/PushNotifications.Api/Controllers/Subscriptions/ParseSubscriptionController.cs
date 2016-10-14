using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Commands;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using Thinktecture.IdentityModel.WebApi;

namespace PushNotifications.Api.Controllers.Subscriptions
{
    [ScopeAuthorize("owner")]
    [RoutePrefix("Subscriptions/ParseSubscription")]
    public class ParseSubscriptionController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        [HttpPost, Route("Subscribe")]
        public IHttpActionResult Subscribe(ParseSubscriptionModel model)
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

        [HttpPost, Route("UnSubscribe")]
        public IHttpActionResult UnSubscribe(ParseSubscriptionModel model)
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

    public class ParseSubscriptionModel
    {
        [AuthorizeClaim(AuthorizeClaimType.Subject)]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        public SubscribeForParse AsSubscribeCommand()
        {
            var id = new ParseSubscriptionId(Token);
            return new SubscribeForParse(id, UserId, Token);
        }

        public UnSubscribeFromParse AsUnSubscribeCommand()
        {
            var id = new ParseSubscriptionId(Token);
            return new UnSubscribeFromParse(id, UserId, Token);
        }
    }
}
