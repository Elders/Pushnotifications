//using Elders.Web.Api;
//using System.Web.Http;
//using PushNotifications.Contracts;
//using Discovery.Contracts;
//using Elders.Cronus.Projections;
//using Elders.Cronus;
//using System.Web.Http.ModelBinding;
//using PushNotifications.Api.Converters;
//using System.ComponentModel.DataAnnotations;
//using System.Collections.Generic;
//using System;
//using PushNotifications.Api.Attributes;
//using PushNotifications.Projections.Subscriptions;
//using PushNotifications.Contracts.Subscriptions;
//using Microsoft.AspNetCore.Mvc;

//namespace PushNotifications.Api.Controllers.Subscriptions.Queries
//{
//    [Route("Subscriptions")]
//    public class GetSubscriberTokensController : ApiControllerBase
//    {
//        public IProjectionReader Projections { get; set; }

//        /// <summary>
//        /// Provides registered tokens for user for all delivery providers e.g FireBase, Pushy etc. Restricted for administrators
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [ScopeAndOrRoleAuthorize(Roles = AvailableRoles.Admin, Scope = AvailableScopes.Admin)]
//        [HttpGet, Route("SubscriberTokens"), Discoverable("SubscriberTokens", "v1")]
//        public IHttpActionResult GetSubscriberTokens(SubscriberTokensModel model)
//        {
//            var subscriberId = new SubscriberId(model.SubscriberUrn.Id, model.SubscriberUrn.Tenant);

//            var projectionReponse = Projections.Get<SubscriberTokensProjection>(subscriberId);
//            if (projectionReponse.Success == true)
//            {
//                return Ok(new ResponseResult<SubscriberTokens>(projectionReponse.Projection.State));
//            }

//            return NotFound();
//        }

//        [ModelBinder(typeof(UrlBinder))]
//        public class SubscriberTokensModel
//        {
//            [Required]
//            public StringTenantUrn SubscriberUrn { get; set; }
//        }
//    }
//}
