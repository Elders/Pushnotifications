using Elders.Discovery;
using Microsoft.AspNetCore.Mvc;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Route("Subscriptions/PushySubscription")]
    public class PushyUnSubscriptionController : ApiControllerBase
    {
        private readonly ApiCqrsResponse response;
        private readonly ApiContext context;

        public PushyUnSubscriptionController(ApiCqrsResponse response, ApiContext context)
        {
            this.response = response;
            this.context = context;
        }

        /// <summary>
        /// UnSubscribes from push notifications with Pushy token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UnSubscribe"), Discoverable("PushySubscriptionUnSubscribe", "v1")]
        public IActionResult UnSubscribeFromPushy(PushySubscribeModel model)
        {
            if (context.CurrentUser.UserId is null && string.IsNullOrEmpty(model.Subscriber))
                return response.ValidationProblem("Please use RO or provide the subscriber property.");

            var command = model.AsUnSubscribeCommand(context);

            return response.FromPublishCommand(command);
        }
    }
}
