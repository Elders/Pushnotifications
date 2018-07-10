﻿using Elders.Cronus;
using Elders.Web.Api;
using System.Web.Http;
using PushNotifications.Contracts;
using Discovery.Contracts;
using System.Collections.Generic;
using System;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [RoutePrefix("Subscriptions/FireBaseSubscription")]
    public class FireBaseTopicSubscribeController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        /// <summary>
        /// Subscribes for push notifications with FireBase token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SubscribeToTopic"), Discoverable("FireBaseSubscriptionSubscribeToTopic", "v1")]
        public IHttpActionResult SubscribeToTopicFireBase(FirebaseTopicSubscriptionModel model)
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

        public class Examples : IProvideRExamplesFor<FireBaseTopicSubscribeController>
        {
            public IEnumerable<IRExample> GetRExamples()
            {
                var tenant = "elders";
                var subscriberId = new SubscriberId(Guid.NewGuid().ToString(), tenant);

                yield return new RExample(new FirebaseTopicSubscriptionModel()
                {
                    SubscriberUrn = StringTenantUrn.Parse(subscriberId.Urn.Value),
                    Token = "token",
                    Topic = "topic"
                });

                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.NotAcceptable, new ResponseResult(Constants.CommandPublishFailed));
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.Accepted, new ResponseResult());
            }
        }
    }
}