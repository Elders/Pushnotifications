using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.Web.Http;
using PushNotifications.Contracts;
using Discovery.Contracts;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Projections;
using PushNotifications.Api.Controllers.Subscriptions.Models;

namespace PushNotifications.Api.Controllers.Subscriptions.Queries
{
    [RoutePrefix("Subscriptions")]
    public class FireBaseSubscriptionQueryController : ApiController
    {
        public IProjectionRepository Projections { get; set; }

        /// <summary>
        /// Provides registered tokens via FireBase for user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet, Route("FireBaseSubscriberTokens"), Discoverable("FireBaseSubscriberTokens", "v1")]
        public IHttpActionResult GetFireBaseSubscriberTokens(SubscriberTokensModel model)
        {
            if (Urn.IsUrn(model.SubscriberId) == false)
                return Ok(new ResponseResult());

            var urn = StringTenantUrn.Parse(model.SubscriberId);
            var subscriberId = new SubscriberId(urn.Id, urn.Tenant);

            var projectionReponse = Projections.Get<SubscriberTokensForFireBaseProjection>(subscriberId);
            if (projectionReponse.Success == true)
            {
                return Ok(new ResponseResult<SubscriberTokens>(projectionReponse.Projection.State));
            }

            return Ok(new ResponseResult());
        }

    }
}
