using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using Thinktecture.IdentityModel.WebApi;
using Discovery;

namespace PushNotifications.Api.Controllers.Subscriptions
{
    [ScopeAuthorize("owner")]
    [RoutePrefix("Subscriptions/FireBaseSubscription")]
    public class FireBaseSubscriptionController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        [HttpPost, Route("Subscribe"), Discoverable("FireBaseSubscriptionSubscribe", "v1")]
        public IHttpActionResult Subscribe(FireBaseSubscriptionModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);
            return this.Accepted("This is not really working. Just placeholder endpoint");
        }

        [HttpPost, Route("UnSubscribe"), Discoverable("FireBaseSubscriptionUnSubscribe", "v1")]
        public IHttpActionResult UnSubscribe(FireBaseSubscriptionModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);
            return this.Accepted("This is not really working. Just placeholder endpoint");
        }
    }

    public class FireBaseSubscriptionModel
    {
        [AuthorizeClaim(AuthorizeClaimType.Subject)]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

    }
}
