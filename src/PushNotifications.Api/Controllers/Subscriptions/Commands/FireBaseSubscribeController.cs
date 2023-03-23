using Elders.Discovery;
using Microsoft.AspNetCore.Mvc;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Route("Subscriptions/FireBaseSubscription")]
    public class FireBaseSubscribeController : ApiControllerBase
    {
        private readonly ApiCqrsResponse response;
        private readonly ApiContext context;

        public FireBaseSubscribeController(ApiCqrsResponse response, ApiContext context)
        {
            this.response = response;
            this.context = context;
        }

        /// <summary>
        /// Subscribes for push notifications with FireBase token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Subscribe"), Discoverable("FireBaseSubscriptionSubscribe", "v1")]
        public IActionResult SubscribeToFireBase(FireBaseSubscribeModel model)
        {
            if (context.CurrentUser.UserId is null && string.IsNullOrEmpty(model.Subscriber))
                return response.ValidationProblem("Please use RO or provide the subscriber property.");

            var command = model.AsSubscribeCommand(context);

            return response.FromPublishCommand(command);
        }
    }
}
