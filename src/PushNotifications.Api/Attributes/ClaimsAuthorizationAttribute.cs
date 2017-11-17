using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace PushNotifications.Api.Attributes
{
    public class ClaimsAuthorizationAttribute : AuthorizeAttribute
    {
        string claimType;
        string claimValue;

        public ClaimsAuthorizationAttribute(string type, string value)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            claimType = type;
            claimValue = value;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var claims = ClaimsPrincipal.Current.FindAll(claimType).ToList();
            var result = claims.Any(x => x.Value == claimValue);

            return result;
        }
    }
}
