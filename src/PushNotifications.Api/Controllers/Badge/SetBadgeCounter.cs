using Discovery.Contracts;
using Elders.Cronus;
using Elders.Web.Api;
using PushNotifications.Api.Attributes;
using PushNotifications.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Web.Http;

namespace PushNotifications.Api.Controllers.Badge
{
    [RoutePrefix("Badge")]
    public class SetBadgeCounterController : ApiController
    {
        public IBadgeCountTrackerFactory BadgeCountTrackerFactory { get; set; }

        /// <summary>
        /// Sets the BadgeCount for a subscriber.
        /// </summary>
        /// <param name="model">Contains a mandatory tenant and subscriberId </param>
        /// <returns></returns>
        [HttpPost, Route("SetBadgeCount"), Discoverable("Badges", "v1")]
        public IHttpActionResult SetBadgeCount(BadgeCounterModel model)
        {
            if (model.BadgeCount < 0) return BadRequest("Badge cannot be a negative");

            try
            {
                var subscriberId = new SubscriberId(model.SubscriberUrn.Id, model.Tenant);
                BadgeCountTrackerFactory.GetService(model.Tenant).SetCount(subscriberId.Urn.Value, model.BadgeCount);
            }
            catch (System.Exception)
            {
                return InternalServerError();
            }

            return Ok(new ResponseResult());
        }

        public class BadgeCounterModel
        {
            [Required]
            [ClaimsIdentity(AuthorizeClaimType.Tenant, AuthorizeClaimType.TenantClient)]
            public string Tenant { get; set; }

            [ClaimsIdentity(AuthorizeClaimType.Subject, ClaimTypes.NameIdentifier)]
            public StringTenantUrn SubscriberUrn { get; set; }

            [Required]
            public int BadgeCount { get; set; }
        }
    }
}
