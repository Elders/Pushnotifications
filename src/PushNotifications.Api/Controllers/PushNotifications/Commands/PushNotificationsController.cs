using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.Web.Http;
using PushNotifications.Contracts;
using PushNotifications.Api.Attributes;
using Discovery.Contracts;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Converters.Extensions;
using System.Collections.Generic;
using System;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Api.Controllers.PushNotifications.Models;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [Scope(AvailableScopes.Admin)]
    [RoutePrefix("PushNotifications")]
    public class PushNotificationsController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        public IProjectionRepository Projections { get; set; }

        /// <summary>
        /// Sends push notification with notification payload. This endpoint is accessable only with admin scope.
        /// Sending of push notification won't be trigger if existing subscription is not found for specified subscriber
        /// Restricted with admin scope
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [ScopeAndOrRoleAuthorize(Roles = AvailableRoles.Admin, Scopes = AvailableScopes.Admin)]
        [HttpPost, Route("Send"), Discoverable("PushNotificationsSend", "v1")]
        public IHttpActionResult Send(SendPushNotificationModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);

            var subscriberId = new SubscriberId(model.SubscriberUrn.Id, model.SubscriberUrn.Tenant);
            var projectionReponse = Projections.Get<SubscriberTokensProjection>(subscriberId);
            if (projectionReponse.Success == false || projectionReponse.Projection.State.Tokens.Count == 0)
            {
                return this.NotAcceptable(new ResponseResult($"Subscription not found for provided subscriber '{model.SubscriberUrn.Value.UrlEncode()}'"));
            }

            var command = model.AsCommand();
            result = Publisher.Publish(command)
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
                    Tenant = tenant,
                    Title = "The title"
                });

                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.NotAcceptable, new ResponseResult(Constants.CommandPublishFailed));
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.Accepted, new ResponseResult());
            }
        }
    }
}
