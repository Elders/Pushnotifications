using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace PushNotifications.Api
{
    [DataContract(Name = "4de4989d-8042-44b0-9d61-50bc83d38236")]
    public class IdentityProblemDetails : ExtendedProblemDetails
    {
        public IdentityProblemDetails(string details, string provider, HttpContext httpContext)
            : base(httpContext)
        {
            Title = "Identity problem with a third party system.";
            Status = StatusCodes.Status400BadRequest;
            Detail = details;
            Extensions["shouldConnect"] = true;
            Extensions["provider"] = provider;
        }
    }
}
