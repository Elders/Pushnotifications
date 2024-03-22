using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace PushNotifications.Api
{
    /// <summary>
    /// Generic wrapper of "ProblemDetails" which is used by the system. Unifies the assignment of 'Type', 'ErrorCodes' and some times 'Instance' values to all types of custom problem details
    /// https://tools.ietf.org/html/rfc7807
    /// </summary>
    /// <seealso cref="ExtendedProblemDetails" />
    public abstract class ExtendedProblemDetails : ProblemDetails
    {
        private const string Namespace = "urn:UniCom.VirtualInventory";
        protected const string ErrorCodeKey = "errorCodeId";

        public ExtendedProblemDetails()
        {
            Type = "about:blank";
            Instance = $"{Namespace}:trace:{Guid.NewGuid()}";
            Extensions[ErrorCodeKey] = GetInstanceErrorCode();
        }

        public ExtendedProblemDetails(HttpContext httpContext) : this()
        {
            Extensions["request"] = new RequestDetails(httpContext);
            Instance = httpContext.Request.Path;
        }

        string GetInstanceErrorCode()
        {
            DataContractAttribute errorCode = GetType()
                .GetCustomAttributes(typeof(DataContractAttribute), false)
                .SingleOrDefault() as DataContractAttribute;

            if (errorCode is null)
                throw new ArgumentException("Every `ProblemDetail` should have an attribute `DataContract` with GUID as name!", nameof(errorCode));

            if (errorCode is null == false)
                return $"{Namespace}:error:{errorCode.Name}";

            return string.Empty;
        }

        class RequestDetails
        {
            public RequestDetails(HttpContext context)
            {
                //context.Connection.RemoteIpAddress is null when running from tests
                if (ReferenceEquals(context.Connection.RemoteIpAddress, null) == false)
                    Source = context.Connection.RemoteIpAddress.ToString();
                else
                    Source = "127.0.0.1";

                HttpRequest request = context.Request;
                if (request is null == false)
                {
                    Resource = $"{request.Method} {request.Path.Value}{request.QueryString.Value}";
                    Headers = request.Headers.ToDictionary(k => k.Key, v => v.Value);

                    if (request.ContentLength > 0)
                    {
                        request.EnableBuffering(); // https://github.com/aspnet/KestrelHttpServer/issues/1548
                        request.Body.Seek(0, SeekOrigin.Begin);
                        using (StreamReader reader = new StreamReader(request.Body))
                        {
                            Body = reader.ReadToEndAsync().GetAwaiter().GetResult();
                        }
                    }
                }
            }

            public string Source { get; set; }

            public string Resource { get; set; }

            public Dictionary<string, StringValues> Headers { get; set; }

            public string Body { get; set; }
        }
    }
}
