using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.Web.Http;
using PushNotifications.Contracts;
using PushNotifications.Api.Attributes;
using Discovery.Contracts;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Projections.General;
using PushNotifications.Converters.Extensions;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Scope(AvailableScopes.Admin)]
    [RoutePrefix("PushNotifications")]
    public class PushNotificationsController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        public IProjectionRepository Projections { get; set; }

        /// <summary>
        /// Sends push notification with notification payload. This endpoint is accessable only with admin scope.
        /// Sending of push notification won't be trigger if existing subscription is not found for specified subscriber
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Send"), Discoverable("PushNotificationsSend", "v1")]
        public IHttpActionResult Send(PushNotificationSendModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);

            var subscriberId = new SubscriberId(model.SubscriberUrn);
            var projectionReponse = Projections.Get<SubscriberTokensForAllProvidersProjection>(subscriberId);
            if (projectionReponse.Success == false || projectionReponse.Projection.State.Tokens.Count == 0)
            {
                return this.NotAcceptable($"Subscription not found for provided subscriber '{model.SubscriberUrn.Value.UrlEncode()}'");
            }

            var command = model.AsCommand();
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
