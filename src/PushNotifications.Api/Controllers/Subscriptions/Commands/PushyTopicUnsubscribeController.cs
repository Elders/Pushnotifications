using Elders.Cronus;
using Elders.Web.Api;
using System.Web.Http;
using Discovery.Contracts;
using System.Collections.Generic;
using PushNotifications.Contracts;
using System;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [RoutePrefix("Subscriptions/PushySubscription")]
    public class PushyTopicUnsubscribeController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        /// <summary>
        /// Unsubscribes a Subscriber from a topic [Pushy]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UnsubscribeFromTopic"), Discoverable("PushySubscriptionUnsubscribeFromTopic", "v1")]
        public IHttpActionResult UnsubscribeFromTopicPushy(PushySubscribeToTopicModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);

            var command = model.AsSubscribeToTopicCommand();
            result = Publisher.Publish(command)
                    ? new ResponseResult<ResponseResult>(new ResponseResult())
                    : new ResponseResult(Constants.CommandPublishFailed);

            return result.IsSuccess
                ? this.Accepted(result)
                : this.NotAcceptable(result);
        }

        public class Examples : IProvideRExamplesFor<PushyTopicUnsubscribeController>
        {
            public IEnumerable<IRExample> GetRExamples()
            {
                var topic = new Topic("test");
                var subscriberId = new SubscriberId(Guid.NewGuid().ToString(), "elders");

                yield return new RExample(new PushySubscribeToTopicModel()
                {
                    SubscriberId = subscriberId,
                    Topic = topic
                });

                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.NotAcceptable, new ResponseResult(Constants.CommandPublishFailed));
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.Accepted, new ResponseResult());
            }
        }
    }
}
