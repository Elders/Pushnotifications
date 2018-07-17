using Elders.Cronus;
using Elders.Web.Api;
using System.Web.Http;
using PushNotifications.Contracts;
using PushNotifications.Api.Attributes;
using Discovery.Contracts;
using System.Collections.Generic;
using System;
using PushNotifications.Api.Controllers.PushNotifications.Models;
using PushNotifications.Contracts.PushNotifications.Events;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Scope(AvailableScopes.Admin)]
    [RoutePrefix("PushNotifications")]
    public class PushNotificationsController : ApiController
    {
        public IPublisher<IEvent> Publisher { get; set; }

        /// <summary>
        /// Sends push notification with notification payload. This endpoint is accessable only with admin scope.
        /// Sending of push notification won't be trigger if existing subscription is not found for specified subscriber
        /// Restricted with admin scope
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ScopeAndOrRoleAuthorize(Roles = AvailableRoles.Admin, Scope = AvailableScopes.Admin)]
        [HttpPost, Route("Send"), Discoverable("PushNotificationsSend", "v1")]
        public IHttpActionResult Send(SendPushNotificationModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);

            var subscriberId = new SubscriberId(model.SubscriberUrn.Id, model.SubscriberUrn.Tenant);

            PushNotificationSent @event = model.AsEvent();
            result = Publisher.Publish(@event, new Dictionary<string, string>() { { "ar_id", Convert.ToBase64String(@event.Id.RawId) }, { "ar_revision", "1" }, { "event_position", "0" } })
                   ? new ResponseResult<ResponseResult>(new ResponseResult())
                   : new ResponseResult(Constants.CommandPublishFailed);

            return result.IsSuccess
                ? this.Accepted(result)
                : this.NotAcceptable(result);
        }

        public class Examples : IProvideRExamplesFor<PushNotificationsController>
        {
            public IEnumerable<IRExample> GetRExamples()
            {
                var tenant = "elders";
                var subscriberId = new SubscriberId(Guid.NewGuid().ToString(), tenant);
                var stringTenantUrn = StringTenantUrn.Parse(subscriberId.Urn.Value);
                yield return new RExample(new SendPushNotificationModel
                {
                    Badge = 0,
                    Body = "test body",
                    ContentAvailable = true,
                    ExpiresAt = Timestamp.UtcNow(),
                    Icon = string.Empty,
                    Sound = "default",
                    SubscriberUrn = stringTenantUrn,
                    Title = "The title",
                    NotificationData = new Dictionary<string, object>()
                });

                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.NotAcceptable, new ResponseResult(Constants.CommandPublishFailed));
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.Accepted, new ResponseResult());
            }
        }
    }
}
