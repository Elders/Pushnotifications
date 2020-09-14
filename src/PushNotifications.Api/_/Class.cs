using Elders.Cronus;
using Elders.Cronus.MessageProcessing;
using Microsoft.AspNetCore.Http;
using PushNotifications.Subscriptions;
using System;
using System.Linq;
using System.Security.Claims;

namespace PushNotifications.Api
{
    public class ApiContext
    {
        private readonly CronusContext cronusContext;

        public ApiContext(IHttpContextAccessor httpContextAccessor, CronusContext cronusContext)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.cronusContext = cronusContext;
        }

        public IHttpContextAccessor HttpContextAccessor { get; }

        public CurrentUser CurrentUser => new CurrentUser(this);

        public string Tenant => cronusContext.Tenant;
    }

    public class CurrentUser
    {
        private readonly ApiContext vaptContext;

        public CurrentUser(ApiContext apiContext)
        {
            this.vaptContext = apiContext;
        }

        public SubscriberId UserId => GetUserIdFromHttpContext();

        public bool HasRole(string role)
        {
            if (string.IsNullOrEmpty(role)) throw new ArgumentNullException(nameof(role));

            return vaptContext.HttpContextAccessor.HttpContext.User.IsInRole(role);
        }

        private SubscriberId GetUserIdFromHttpContext()
        {
            var claim = vaptContext.HttpContextAccessor.HttpContext.User.Claims
                .Where(c => c.Type.Equals(AuthorizeClaimType.Subject, StringComparison.OrdinalIgnoreCase) || c.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefault();

            if (claim is null)
            {
                return SubscriberId.NoUser;
            }


            if (Urn.IsUrn(claim.Value))
            {
                return new SubscriberId(AggregateUrn.Parse(claim.Value).Id, vaptContext.Tenant);
            }
            else
            {
                return new SubscriberId(claim.Value, vaptContext.Tenant);
            }
        }
    }

    public static class AuthorizeClaimType
    {
        public const string Subject = "sub";
        public const string Tenant = "tenant";
        public const string TenantClient = "client_tenant";
        public const string Organizations = "organizations";
    }
}
