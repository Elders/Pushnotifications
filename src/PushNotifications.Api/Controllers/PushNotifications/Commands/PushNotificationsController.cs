using PushNotifications.Api.Controllers.PushNotifications.Models;
using Microsoft.AspNetCore.Mvc;
using Elders.Discovery;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Route("PushNotifications")]
    public class PushNotificationsController : ApiControllerBase
    {
        private readonly ApiCqrsResponse response;

        public PushNotificationsController(ApiCqrsResponse response)
        {
            this.response = response;
        }

        /// <summary>
        /// Sends push notification with notification payload. This endpoint is accessable only with admin scope.
        /// Sending of push notification won't be trigger if existing subscription is not found for specified subscriber
        /// Restricted with admin scope
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Send"), Discoverable("PushNotificationsSend", "v1")]
        public IActionResult Send(SendPushNotificationModel model)
        {
            //model.Subscriber = model.Subscriber ?? context.CurrentUser.UserId;
            var command = model.AsSignal();

            return response.FromPublishSignal(command);
        }
    }
}
