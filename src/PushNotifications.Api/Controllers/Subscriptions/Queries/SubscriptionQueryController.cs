using Elders.Web.Api;
using System.Web.Http;
using PushNotifications.Contracts;
using Discovery.Contracts;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Projections;
using Elders.Cronus.DomainModeling;
using System.Web.Http.ModelBinding;
using PushNotifications.Api.Converters;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using PushNotifications.Api.Attributes;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Contracts.Subscriptions;

namespace PushNotifications.Api.Controllers.Subscriptions.Queries
{
    [RoutePrefix("Subscriptions")]
    public class GetSubscriberTokensController : ApiController
    {
        public IProjectionRepository Projections { get; set; }

        /// <summary>
        /// Provides registered tokens for user for all delivery providers e.g FireBase, Pushy etc. Restricted for administrators
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UsefulAuthorize(Roles = AvailableRoles.Admin, Scopes = AvailableScopes.Admin)]
        [HttpGet, Route("SubscriberTokens"), Discoverable("SubscriberTokens", "v1")]
        public IHttpActionResult GetSubscriberTokens(SubscriberTokensModel model)
        {
            var subscriberId = new SubscriberId(model.SubscriberUrn.Id, model.SubscriberUrn.Tenant);

            var projectionReponse = Projections.Get<SubscriberTokensProjection>(subscriberId);
            if (projectionReponse.Success == true)
            {
                return Ok(new ResponseResult<SubscriberTokens>(projectionReponse.Projection.State));
            }

            return NotFound();
        }

        [ModelBinder(typeof(UrlBinder))]
        public class SubscriberTokensModel
        {
            [Required]
            public StringTenantUrn SubscriberUrn { get; set; }
        }

        public class Examples : IProvideRExamplesFor<GetSubscriberTokensController>
        {
            public IEnumerable<IRExample> GetRExamples()
            {
                var tenant = "elders";
                var subscriberId = new SubscriberId(Guid.NewGuid().ToString(), tenant);
                var stringTenantUrn = StringTenantUrn.Parse(subscriberId.Urn.Value);
                yield return new RExample(new SubscriberTokensModel { SubscriberUrn = stringTenantUrn });

                var tokens = new HashSet<SubscriptionToken>
                {
                         new SubscriptionToken("t1",SubscriptionType.FireBase),
                         new SubscriptionToken("t2",SubscriptionType.Pushy)
                };
                var model = new SubscriberTokens(subscriberId, tokens);

                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.NotFound, "This is null. Oh well should be null.");
                yield return new Elders.Web.Api.RExamples.StatusRExample(System.Net.HttpStatusCode.OK, new ResponseResult<SubscriberTokens>(model));
            }
        }
    }
}
