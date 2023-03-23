using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PushNotifications.Api
{
    /// <summary>
    /// Whenever  an error happens while trying to load a projection this problem should be returned
    /// </summary>
    /// <seealso cref="ExtendedProblemDetails" />
    [DataContract(Name = "b30e83cc-03b2-48e4-acbf-627818807b7c")]
    public class ProjectionProblemDetails : ExtendedProblemDetails
    {
        public ProjectionProblemDetails(HttpContext context, IEnumerable<string> errors) : base(context)
        {
            Title = "Something happened while loading your information";
            Detail = string.Join("; ", errors);
            Status = StatusCodes.Status500InternalServerError;
        }
    }
}
