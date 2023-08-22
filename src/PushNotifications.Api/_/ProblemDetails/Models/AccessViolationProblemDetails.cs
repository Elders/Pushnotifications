using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace PushNotifications.Api
{
    /// <summary>Details Problem Object used for problems related to the validation of an incoming request</summary>
    /// <seealso cref="ExtendedProblemDetails" />
    [DataContract(Name = "6b380086-a2f8-4799-bfaa-35ae22f86927")]
    public class AccessViolationProblemDetails : ExtendedProblemDetails
    {
        public AccessViolationProblemDetails(HttpContext httpContext, string details)
            : base(httpContext)
        {
            Title = "Invalid request data provided by the client!";
            Detail = details;
            Status = StatusCodes.Status403Forbidden;
        }
    }

    [DataContract(Name = "477a2b0c-34ee-45e2-9859-6ed08b8838ea")]
    public class UnknownProblemDetails : ExtendedProblemDetails
    {
        public UnknownProblemDetails(HttpContext httpContext, string details)
            : base(httpContext)
        {
            Title = "Failed to execute request due to some unknown reason.";
            Detail = details;
            Status = StatusCodes.Status403Forbidden;
        }
    }
}
