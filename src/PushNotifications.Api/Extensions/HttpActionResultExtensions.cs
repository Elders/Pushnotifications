using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace PushNotifications.Api.Extensions
{
    public static class HttpActionResultExtensions
    {
        public static IHttpActionResult SetLastModifiedHeader(this IHttpActionResult actionResult, DateTime lastModified)
        {
            var x = new LastModifiedHeaderActionResult(actionResult);
            x.AddLastModofied(lastModified);

            return x;
        }

        class LastModifiedHeaderActionResult : IHttpActionResult
        {
            readonly IHttpActionResult actionResult;

            DateTime lastModified;

            public LastModifiedHeaderActionResult(IHttpActionResult actionResult)
            {
                this.actionResult = actionResult;
            }

            public void AddLastModofied(DateTime lastModified)
            {
                this.lastModified = lastModified;
            }

            public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage httpResponseMessage = await this.actionResult.ExecuteAsync(cancellationToken);

                if (lastModified != default(DateTime))
                    httpResponseMessage.Content.Headers.LastModified = lastModified;

                return httpResponseMessage;
            }
        }
    }
}
