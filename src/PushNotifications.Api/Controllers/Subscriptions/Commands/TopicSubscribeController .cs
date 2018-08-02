using Elders.Cronus;
using Elders.Web.Api;
using System.Web.Http;
using PushNotifications.Contracts;
using Discovery.Contracts;
using System.Collections.Generic;
using System;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [RoutePrefix("Subscriptions")]
    public class TopicSubscribeController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        /// <summary>
        /// Subscribes a Subscriber for push notifications for a topic 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SubscribeToTopic"), Discoverable("TopicSubscriptionSubscribeToTopic", "v1")]
        public IHttpActionResult SubscribeToTopic(TopicSubscriptionModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);

            var command = model.AsSubscribeToTopicCommand();
            result = Publisher.Publish(command)
                   ? new ResponseResult()
                   : new ResponseResult(Constants.CommandPublishFailed);

            return result.IsSuccess
                ? this.Accepted(result)
                : this.NotAcceptable(result);
        }

        public class Examples : IProvideRExamplesFor<TopicSubscribeController>
        {
            public IEnumerable<IRExample> GetRExamples()
            {
                var tenant = "elders";
                var subscriberId = new SubscriberId(Guid.NewGuid().ToString(), tenant);

                yield return new RExample(new TopicSubscriptionModel()
                {
                    SubscriberId = subscriberId,
                    Topic = new Topic("topic")
                });

                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.NotAcceptable, new ResponseResult(Constants.CommandPublishFailed));
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.Accepted, new ResponseResult());
            }
        }
    }
}
