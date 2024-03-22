using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace PushNotifications.Api
{
    /// <summary>
    /// This problem should be used each time the resource which is requested, for some reason, would not be found and returned
    /// </summary>
    /// <seealso cref="ExtendedProblemDetails" />
    [DataContract(Name = "de43e535-7ff2-49ff-bba3-1da33031b5e0")]
    public class ResourceNotFoundProblemDetails : ExtendedProblemDetails
    {
        public ResourceNotFoundProblemDetails(HttpContext context, string detailsMessage) : base(context)
        {
            Title = "The resource you are looking for cannot be found!";
            Detail = detailsMessage;
            Status = StatusCodes.Status404NotFound;
        }
    }
}
