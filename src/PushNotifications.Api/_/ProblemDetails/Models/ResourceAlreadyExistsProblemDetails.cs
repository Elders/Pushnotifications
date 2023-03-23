using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace PushNotifications.Api
{
    /// <summary>
    /// This problem should be used each time the resource which is requested for creation, already exists
    /// </summary>
    /// <seealso cref="ExtendedProblemDetails" />
    [DataContract(Name = "61ef7432-706e-4ff5-8b36-9cdef3f6296d")]
    public class ResourceAlreadyExistsProblemDetails : ExtendedProblemDetails
    {
        public ResourceAlreadyExistsProblemDetails(HttpContext context, string detailsMessage, string existingResourceId) : base(context)
        {
            Title = "Resource already exists.";
            Detail = detailsMessage;
            Status = StatusCodes.Status409Conflict;
            Extensions["ExistingResourceId"] = existingResourceId;
        }
    }
}
