using Elders.Discovery;
using Microsoft.AspNetCore.Mvc;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Route("Subscriptions")]
    public class TopicSubscribeController : ApiControllerBase
    {
        private readonly ApiCqrsResponse response;

        public TopicSubscribeController(ApiCqrsResponse response)
        {
            this.response = response;
        }

        /// <summary>
        /// Subscribes a Subscriber for push notifications for a topic 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SubscribeToTopic"), Discoverable("TopicSubscriptionSubscribeToTopic", "v1")]
        public IActionResult SubscribeToTopic(TopicSubscriptionModel model)
        {
            var command = model.AsSubscribeToTopicCommand();

            return response.FromPublishCommand(command);
        }
    }
}
