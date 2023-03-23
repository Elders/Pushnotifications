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
            if (context.CurrentUser.UserId is null && string.IsNullOrEmpty(model.Subscriber))
                return response.ValidationProblem("Please use RO or provide the subscriber property.");

            var command = model.AsSubscribeCommand(context);

            return response.FromPublishCommand(command);
        }
    }
}
