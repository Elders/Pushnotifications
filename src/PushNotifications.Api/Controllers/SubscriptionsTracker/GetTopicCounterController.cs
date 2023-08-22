//using Discovery.Contracts;
//using Elders.Discovery;
//using Elders.Web.Api;
//using Microsoft.AspNetCore.Components;
//using PushNotifications.Api.Attributes;
//using PushNotifications.Api.Converters;
//using PushNotifications.Contracts;
//using System.ComponentModel.DataAnnotations;
//using System.Web.Http;
//using System.Web.Http.ModelBinding;

//namespace PushNotifications.Api.Controllers.SubscriptionsTracker
//{
//    [Route("TopicSubscriptionCount")]
//    public class GetTopicCounterController : ApiControllerBase
//    {
//        public ITopicSubscriptionTrackerFactory TopicSubscriptionTracker { get; set; }

//        /// <summary>
//        /// This endpoint is here in order to be able to track if the subscriptions to topics are correct.
//        /// Furthermore we can easily track how many and what topics have been generated
//        /// </summary>
//        /// <param name="model">Contains a mandatory tenant and name which identifies the topic</param>
//        /// <returns></returns>
//        [HttpGet, Route("GetTopicSubscribedCount"), Discoverable("TopicSubscriptionCount", "v1")]
//        public IHttpActionResult GetTopicSubscribedCount(TopicCounter model)
//        {
//            StatCounter result = TopicSubscriptionTracker.GetService(model.Tenant).Show(model.Name);

//            return Ok(new ResponseResult<StatCounter>(result));
//        }

//        [ModelBinder(typeof(UrlBinder))]
//        public class TopicCounter
//        {
//            [Required]
//            [ClaimsIdentity(AuthorizeClaimType.Tenant, AuthorizeClaimType.TenantClient)]
//            public string Tenant { get; set; }

//            [Required]
//            public string Name { get; set; }
//        }
//    }
//}
