using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.Web.Http;
using Discovery.Contracts;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [RoutePrefix("Subscriptions/PushySubscription")]
    public class PushySubscriptionCommandController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        /// <summary>
        /// Subscribes for push notifications with Pushy token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Subscribe"), Discoverable("PushySubscriptionSubscribe", "v1")]
        public IHttpActionResult SubscribeToPushy(PushySubscribeModel model)
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
        /// UnSubscribes from push notifications with Pushy token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UnSubscribe"), Discoverable("PushySubscriptionUnSubscribe", "v1")]
        public IHttpActionResult UnSubscribeFromPushy(PushySubscribeModel model)
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
}
