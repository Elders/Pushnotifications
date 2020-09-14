using Microsoft.AspNetCore.Mvc;
using Elders.Discovery;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Route("Subscriptions/PushySubscription")]
    public class PushySubscriptionController : ApiControllerBase
    {
        private readonly ApiCqrsResponse response;
        private readonly ApiContext context;

        public PushySubscriptionController(ApiCqrsResponse response, ApiContext context)
        {
            this.response = response;
            this.context = context;
        }

        /// <summary>
        /// Subscribes for push notifications with Pushy token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Subscribe"), Discoverable("PushySubscriptionSubscribe", "v1")]
        public IActionResult SubscribeToPushy(PushySubscribeModel model)
        {
            model.Subscriber = model.Subscriber ?? context.CurrentUser.UserId;
            var command = model.AsSubscribeCommand();

            return response.FromPublishCommand(command);
        }
    }
}
