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
    public class FireBaseSubscribeController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        /// <summary>
        /// Subscribes for push notifications with FireBase token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Subscribe"), Discoverable("FireBaseSubscriptionSubscribe", "v1")]
        public IHttpActionResult SubscribeToFireBase(FireBaseSubscribeModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);

            var command = model.AsSubscribeCommand();
            result = Publisher.Publish(command)
                   ? new ResponseResult()
                   : new ResponseResult(Constants.CommandPublishFailed);

            return result.IsSuccess
                ? this.Accepted(result)
                : this.NotAcceptable(result);
        }

        public class Examples : IProvideRExamplesFor<FireBaseSubscribeController>
        {
            public IEnumerable<IRExample> GetRExamples()
            {
                var tenant = "elders";
                var subscriberId = new SubscriberId(Guid.NewGuid().ToString(), tenant);

                yield return new RExample(new FireBaseSubscribeModel()
                {
                    SubscriberUrn = StringTenantUrn.Parse(subscriberId.Urn.Value),
                    Token = "token"
                });

                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.NotAcceptable, new ResponseResult(Constants.CommandPublishFailed));
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.Accepted, new ResponseResult());
            }
        }
    }
}
