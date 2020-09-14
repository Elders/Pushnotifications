using Microsoft.AspNetCore.Mvc;
using Elders.Discovery;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Route("Subscriptions/FireBaseSubscription")]
    public class FireBaseUnSubscribeController : ApiControllerBase
    {
        private readonly ApiCqrsResponse response;
        private readonly ApiContext context;

        public FireBaseUnSubscribeController(ApiCqrsResponse response, ApiContext context)
        {
            this.response = response;
            this.context = context;
        }

        /// <summary>
        /// UnSubscribes from push notifications with FireBase token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UnSubscribe"), Discoverable("FireBaseSubscriptionUnSubscribe", "v1")]
        public IActionResult UnSubscribeFromFireBase(FireBaseSubscribeModel model)
        {
            var command = model.AsUnSubscribeCommand(context);

            return response.FromPublishCommand(command);
        }
    }
}
