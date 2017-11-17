using Elders.Web.Api;
using System.Web.Http;
using PushNotifications.Contracts;
using Discovery.Contracts;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Projections;
using PushNotifications.Projections.Pushy;
using System.Collections.Generic;
using Elders.Cronus.DomainModeling;
using System;
using System.Web.Http.ModelBinding;
using PushNotifications.Api.Converters;
using System.ComponentModel.DataAnnotations;
using PushNotifications.Api.Attributes;

namespace PushNotifications.Api.Controllers.Subscriptions.Queries
{
    [RoutePrefix("Subscriptions")]
    public class PushySubscriptionQueryController : ApiController
    {
        public IProjectionRepository Projections { get; set; }

        /// <summary>
        /// Provides registered tokens via Pushy for user. Restricted for administrators
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UsefulAuthorize(Roles = AvailableRoles.Admin, Scopes = AvailableScopes.Admin)]
        [HttpGet, Route("PushySubscriberTokens"), Discoverable("PushySubscriberTokens", "v1")]
        public IHttpActionResult GetPushySubscriberTokens(GetPushySubscriberTokensModel model)
        {
            var subscriberId = new SubscriberId(model.SubscriberUrn.Id, model.SubscriberUrn.Tenant);

            var projectionReponse = Projections.Get<SubscriberTokensForPushyProjection>(subscriberId);
            if (projectionReponse.Success == true)
            {
                return Ok(new ResponseResult<SubscriberTokens>(projectionReponse.Projection.State));
            }

            return NotFound();
        }

        [ModelBinder(typeof(UrlBinder))]
        public class GetPushySubscriberTokensModel
        {
            [Required]
            public StringTenantUrn SubscriberUrn { get; set; }
        }

        public class Examples : IProvideRExamplesFor<PushySubscriptionQueryController>
        {
            public IEnumerable<IRExample> GetRExamples()
            {
                var tenant = "elders";
                var subscriberId = new SubscriberId(Guid.NewGuid().ToString(), tenant);
                var stringTenantUrn = StringTenantUrn.Parse(subscriberId.Urn.Value);
                yield return new RExample(new GetPushySubscriberTokensModel { SubscriberUrn = stringTenantUrn });

                var model = new SubscriberTokens()
                {
                    SubscriberId = subscriberId,
                    Tokens = new HashSet<SubscriptionToken> { new SubscriptionToken("t1"), new SubscriptionToken("t2") }
                };
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.NotFound, "This is null. Oh well should be null.");
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.OK, new ResponseResult<SubscriberTokens>(model));
            }
        }
    }
}
