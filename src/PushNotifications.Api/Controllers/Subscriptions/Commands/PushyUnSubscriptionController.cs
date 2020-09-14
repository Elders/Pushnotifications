using Elders.Discovery;
using Microsoft.AspNetCore.Mvc;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Route("Subscriptions/PushySubscription")]
    public class PushyUnSubscriptionController : ApiControllerBase
    {
        private readonly ApiCqrsResponse response;

        public PushyUnSubscriptionController(ApiCqrsResponse response)
        {
            this.response = response;
        }

        /// <summary>
        /// UnSubscribes from push notifications with Pushy token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UnSubscribe"), Discoverable("PushySubscriptionUnSubscribe", "v1")]
        public IActionResult UnSubscribeFromPushy(PushySubscribeModel model)
        {
            var command = model.AsUnSubscribeCommand();

            return response.FromPublishCommand(command);
        }
    }
}
