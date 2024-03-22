using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PushNotifications.Api
{
    /// <summary>Details Problem Object used for problems related to the validation of an incoming request</summary>
    /// <seealso cref="ExtendedProblemDetails" />
    [DataContract(Name = "2921e3cb-d973-41e2-b350-a5eabe892c81")]
    public class ValidationProblemDetails : ExtendedProblemDetails
    {
        public ValidationProblemDetails(HttpContext httpContext, string details, ICollection<ValidationError> validationErrors)
            : this(httpContext, details)
        {
            Extensions["validationErrors"] = validationErrors;
        }

        public ValidationProblemDetails(HttpContext httpContext, string details)
            : base(httpContext)
        {
            Title = "Invalid request data provided by the client!";
            Detail = details;
            Status = StatusCodes.Status400BadRequest;
        }
    }

    public class ValidationError
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
