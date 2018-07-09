﻿using Elders.Cronus;
using Elders.Web.Api;
using System.Web.Http;
using PushNotifications.Api.Attributes;
using Discovery.Contracts;
using PushNotifications.Api.Controllers.PushNotifications.Models;
using System.Collections.Generic;
using PushNotifications.Contracts;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Scope(AvailableScopes.Admin)]
    [RoutePrefix("PushNotifications")]
    public class PushNotificationsTopicsController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        /// <summary>
        /// Sends push notification with notification payload. This endpoint is accessable only with admin scope.
        /// This sends a push notification to a given topic. Every device subscribed to a topic will be delivered a push notification.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ScopeAndOrRoleAuthorize(Roles = AvailableRoles.Admin, Scope = AvailableScopes.Admin)]
        [HttpPost, Route("SendToTopic"), Discoverable("PushNotificationsTopicsSendToTopic", "v1")]
        public IHttpActionResult SendToTopic(SendPushNotificationToTopicModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);

            var command = model.AsCommand();
            result = Publisher.Publish(command)
                   ? new ResponseResult<ResponseResult>(new ResponseResult())
                   : new ResponseResult(Constants.CommandPublishFailed);

            return result.IsSuccess
                ? this.Accepted(result)
                : this.NotAcceptable(result);
        }
    }

    public class Examples : IProvideRExamplesFor<PushNotificationsTopicsController>
    {
        public IEnumerable<IRExample> GetRExamples()
        {
            var tenant = "elders";
            yield return new RExample(new SendPushNotificationToTopicModel
            {
                Badge = 0,
                Body = "test body",
                ContentAvailable = true,
                ExpiresAt = Timestamp.UtcNow(),
                Icon = string.Empty,
                Sound = "default",
                Tenant = tenant,
                Topic = "topic",
                Title = "The title",
                NotificationData = new Dictionary<string, object>()
            });

            yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.NotAcceptable, new ResponseResult(Constants.CommandPublishFailed));
            yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.Accepted, new ResponseResult());
        }
    }
}