﻿using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.Web.Http;
using Discovery.Contracts;
using System.Collections.Generic;
using PushNotifications.Contracts.PushySubscriptions;
using PushNotifications.Contracts;
using System;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [RoutePrefix("Subscriptions/PushySubscription")]
    public class PushyUnSubscriptionController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        /// <summary>
        /// UnSubscribes from push notifications with Pushy token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UnSubscribe"), Discoverable("PushySubscriptionUnSubscribe", "v1")]
        public IHttpActionResult UnSubscribeFromPushy(PushySubscribeModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);
            if (Urn.IsUrn(model.SubscriberUrn) == false) return this.NotAcceptable($"{nameof(model.SubscriberUrn)} must be URN.");

            var command = model.AsUnSubscribeCommand();
            if (command.IsValid())
            {
                result = Publisher.Publish(command)
                    ? new ResponseResult<ResponseResult>(new ResponseResult())
                    : new ResponseResult(Constants.CommandPublishFailed);
            }
            return result.IsSuccess
                ? this.Accepted(result)
                : this.NotAcceptable(result);
        }

        public class Examples : IProvideRExamplesFor<PushyUnSubscriptionController>
        {
            public IEnumerable<IRExample> GetRExamples()
            {
                var tenant = "elders";
                var subscriberId = new SubscriberId(Guid.NewGuid().ToString(), tenant);
                var fireBaseSubscriptionId = new PushySubscriptionId(Guid.NewGuid().ToString(), tenant);

                yield return new RExample(new PushySubscribeModel()
                {
                    Tenant = tenant,
                    SubscriberUrn = StringTenantUrn.Parse(subscriberId.Urn.Value),
                    Token = new SubscriptionToken("token")
                });

                var result = new ResponseResult(Constants.InvalidCommand);
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.NotAcceptable, new ResponseResult(Constants.InvalidCommand));
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.Accepted, new ResponseResult());
            }
        }
    }
}