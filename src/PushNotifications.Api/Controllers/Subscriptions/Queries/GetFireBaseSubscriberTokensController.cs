using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.Web.Http;
using PushNotifications.Contracts;
using Discovery.Contracts;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Projections;
using PushNotifications.Projections.FireBase;
using System.Collections.Generic;
using System;
using System.Web.Http.ModelBinding;
using PushNotifications.Api.Converters;
using System.ComponentModel.DataAnnotations;
using PushNotifications.Api.Attributes;
using System.Security.Claims;

namespace PushNotifications.Api.Controllers.Subscriptions.Queries
{
    [RoutePrefix("Subscriptions")]
    public class GetFireBaseSubscriberTokensController : ApiController
    {
        public IProjectionRepository Projections { get; set; }

        /// <summary>
        /// Provides registered tokens via FireBase for user. Restricted for administrators
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ClaimsAuthorization(ClaimTypes.Role, AvailableRoles.Admin)]
        [HttpGet, Route("FireBaseSubscriberTokens"), Discoverable("FireBaseSubscriberTokens", "v1")]
        public IHttpActionResult GetFireBaseSubscriberTokens(GetFireBaseSubscriberTokensModel model)
        {
            var subscriberId = new SubscriberId(model.SubscriberUrn.Id, model.SubscriberUrn.Tenant);

            var projectionReponse = Projections.Get<SubscriberTokensForFireBaseProjection>(subscriberId);
            if (projectionReponse.Success == true)
            {
                return Ok(new ResponseResult<SubscriberTokens>(projectionReponse.Projection.State));
            }

            return NotFound();
        }

        public class Examples : IProvideRExamplesFor<GetFireBaseSubscriberTokensController>
        {
            public IEnumerable<IRExample> GetRExamples()
            {
                var tenant = "elders";
                var subscriberId = new SubscriberId(Guid.NewGuid().ToString(), tenant);
                var stringTenantUrn = StringTenantUrn.Parse(subscriberId.Urn.Value);
                yield return new RExample(new GetFireBaseSubscriberTokensModel { SubscriberUrn = stringTenantUrn });

                var model = new SubscriberTokens()
                {
                    SubscriberId = subscriberId,
                    Tokens = new HashSet<SubscriptionToken> { new SubscriptionToken("t1"), new SubscriptionToken("t2") }
                };
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.NotFound, "This is null. Oh well should be null.");
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.OK, new ResponseResult<SubscriberTokens>(model));
            }
        }

        [ModelBinder(typeof(UrlBinder))]
        public class GetFireBaseSubscriberTokensModel
        {
            [Required]
            public StringTenantUrn SubscriberUrn { get; set; }
        }
    }
}
