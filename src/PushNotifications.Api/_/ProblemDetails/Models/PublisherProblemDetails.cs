using Elders.Cronus;
using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace PushNotifications.Api
{
    /// <summary>
    /// This problem should be used every time there is a problem with publishing a command
    /// </summary>
    /// <seealso cref="ExtendedProblemDetails" />
    [DataContract(Name = "a7e7ebdf-0dcf-47e5-9dfc-080440afe6f2")]
    public class PublisherProblemDetails : ExtendedProblemDetails
    {
        public PublisherProblemDetails(HttpContext context, IMessage message) : base(context)
        {
            Title = "Something happened. Please try again later!";
            Detail = $"Message cannot be published: {message.GetType().FullName}";
            Status = StatusCodes.Status500InternalServerError;
        }
    }
}
