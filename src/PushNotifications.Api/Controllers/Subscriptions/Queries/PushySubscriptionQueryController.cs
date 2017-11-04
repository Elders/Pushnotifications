using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.Web.Http;
using PushNotifications.Contracts;
using Discovery.Contracts;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Projections;
using PushNotifications.Api.Controllers.Subscriptions.Models;
using PushNotifications.Projections.Pushy;

namespace PushNotifications.Api.Controllers.Subscriptions.Queries
{
    [RoutePrefix("Subscriptions")]
    public class PushySubscriptionQueryController : ApiController
    {
        public IProjectionRepository Projections { get; set; }

        /// <summary>
        /// Provides registered tokens via Pushy for user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet, Route("PushySubscriberTokens"), Discoverable("PushySubscriberTokens", "v1")]
        public IHttpActionResult GetPushySubscriberTokens(SubscriberTokensModel model)
        {
            if (Urn.IsUrn(model.SubscriberId) == false)
                return Ok(new ResponseResult());

            var urn = StringTenantUrn.Parse(model.SubscriberId);
            var subscriberId = new SubscriberId(urn.Id, urn.Tenant);

            var projectionReponse = Projections.Get<SubscriberTokensForPushyProjection>(subscriberId);
            if (projectionReponse.Success == true)
            {
                return Ok(new ResponseResult<SubscriberTokens>(projectionReponse.Projection.State));
            }

            return Ok(new ResponseResult());
        }
    }
}
