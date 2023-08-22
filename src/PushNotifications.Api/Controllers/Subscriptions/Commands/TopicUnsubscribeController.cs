using Elders.Discovery;
using Microsoft.AspNetCore.Mvc;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Route("Subscriptions")]
    public class TopicUnsubscribeController : ApiControllerBase
    {
        private readonly ApiCqrsResponse response;

        public TopicUnsubscribeController(ApiCqrsResponse response)
        {
            this.response = response;
        }

        /// <summary>
        /// Unsubscribes a SubscriberId for push notifications from a topic [Firebase]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UnsubscribeFromTopic"), Discoverable("TopicSubscriptionUnsubscribeFromTopic", "v1")]
        public IActionResult UnsubscribeFromTopic(TopicSubscriptionModel model)
        {
            var command = model.AsUnSubscribeFromTopicCommand();

            return response.FromPublishCommand(command);
        }
    }
}
