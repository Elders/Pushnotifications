using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using PushNotifications.Contracts.FireBaseSubscriptions.Commands;
using PushNotifications.Contracts.FireBaseSubscriptions;
using PushNotifications.Contracts;
using PushNotifications.Api.Attributes;
using System.Security.Claims;
using Discovery.Contracts;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Projections;
using System.Web.Http.ModelBinding;
using PushNotifications.Api.Converters;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    [RoutePrefix("Subscriptions")]
    public class SubscriptionController : ApiController
    {
        public IProjectionRepository Projections { get; set; }

        /// <summary>
        /// Provides registered tokens for user for all delivery providers e.g FireBase, Pushy etc
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet, Route("SubscriberTokens"), Discoverable("SubscriberTokens", "v1")]
        public IHttpActionResult GetSubscriberTokens(SubscriberTokensModel model)
        {
            if (Urn.IsUrn(model.SubscriberId) == false)
                return Ok(new ResponseResult());

            var urn = StringTenantUrn.Parse(model.SubscriberId);
            var subscriberId = new SubscriberId(urn.Id, urn.Tenant);

            var projectionReponse = Projections.Get<SubscriberTokensProjection>(subscriberId);
            if (projectionReponse.Success == true)
            {
                return Ok(new ResponseResult<SubscriberTokens>(projectionReponse.Projection.State));
            }

            return Ok(new ResponseResult());
        }

    }

    [ModelBinder(typeof(UrlBinder))]
    public class SubscriberTokensModel
    {
        [AuthorizeClaim(AuthorizeClaimType.Tenant, AuthorizeClaimType.TenantClient)]
        public string Tenant { get; set; }

        [ClaimsIdentity(AuthorizeClaimType.Subject, ClaimTypes.NameIdentifier)]
        public string SubscriberId { get; set; }
    }
}
