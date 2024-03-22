using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace PushNotifications.Api
{
    /// <summary>
    /// Whenever something extraordinary for the system happens which is not part of the normal work flow which could be handled, an InternalServerError 500 is returned as a response
    /// </summary>
    /// <seealso cref="ExtendedProblemDetails" />
    [DataContract(Name = "f60ca924-f578-427b-a09b-f1a30425e652")]
    public class InternalServerProblemDetails : ExtendedProblemDetails
    {
        public InternalServerProblemDetails(Exception exception, bool isProduction)
        {
            Title = "An unexpected error occurred!";
            Status = StatusCodes.Status500InternalServerError;
            Detail = isProduction ? $"Please provide the following id `{Extensions[ErrorCodeKey]}` to identify the problem when calling customer support!" : exception.ToStringDemystified();
        }

        public InternalServerProblemDetails(Exception exception, HttpContext httpContext, bool isProduction)
            : base(httpContext)
        {
            Title = "An unexpected error occurred!";
            Status = StatusCodes.Status500InternalServerError;
            Detail = isProduction ? $"Please provide the following id `{Extensions[ErrorCodeKey]}` to identify the problem when calling customer support!" : exception.ToStringDemystified();
        }

        public InternalServerProblemDetails(string errorMessage)
        {
            Title = "An unexpected error occurred!";
            Status = StatusCodes.Status500InternalServerError;
            Detail = errorMessage;
        }
    }
}
