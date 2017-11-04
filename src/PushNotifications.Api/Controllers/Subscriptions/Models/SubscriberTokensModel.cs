using Elders.Web.Api;
using PushNotifications.Api.Attributes;
using System.Security.Claims;
using System.Web.Http.ModelBinding;
using PushNotifications.Api.Converters;

namespace PushNotifications.Api.Controllers.Subscriptions.Models
{
    [ModelBinder(typeof(UrlBinder))]
    public class SubscriberTokensModel
    {
        [AuthorizeClaim(AuthorizeClaimType.Tenant, AuthorizeClaimType.TenantClient)]
        public string Tenant { get; set; }

        [ClaimsIdentity(AuthorizeClaimType.Subject, ClaimTypes.NameIdentifier)]
        public string SubscriberId { get; set; }
    }
}
