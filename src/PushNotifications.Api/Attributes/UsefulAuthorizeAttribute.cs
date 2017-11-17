using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace PushNotifications.Api.Attributes
{
    public class UsefulAuthorizeAttribute : AuthorizeAttribute
    {
        public string Scopes { get; set; }

        public bool CombineScopesAndRoles { get; set; }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var providedScopes = ClaimsPrincipal.Current.FindAll("scope").Select(x => x.Value);
            var providedRoles = ClaimsPrincipal.Current.FindAll("role").Select(x => x.Value);

            var requiredScopes = (Scopes ?? string.Empty).Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var requiredRoles = (Roles ?? string.Empty).Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var hasRequiredScopes = requiredScopes.Any() && requiredScopes.All(x => providedScopes.Contains(x));
            var hasRequiredRoles = requiredRoles.Any() && requiredRoles.All(x => providedRoles.Contains(x));

            if (CombineScopesAndRoles)
            {
                return hasRequiredScopes && hasRequiredRoles;
            }

            return hasRequiredScopes || hasRequiredRoles;
        }
    }
}
