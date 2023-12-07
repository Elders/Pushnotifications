using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace PushNotifications.Api
{
    public static class ActionResultExtensions
    {
        public static IActionResult SetLastModifiedHeader(this IActionResult actionResult, DateTimeOffset lastModified)
        {
            var x = new LastModifiedHeaderActionResult(actionResult);
            x.AddLastModofied(lastModified);

            return x;
        }

        class LastModifiedHeaderActionResult : IActionResult
        {
            readonly IActionResult actionResult;

            DateTimeOffset lastModified;

            public LastModifiedHeaderActionResult(IActionResult actionResult)
            {
                this.actionResult = actionResult;
            }

            public void AddLastModofied(DateTimeOffset lastModified)
            {
                this.lastModified = lastModified;
            }

            public Task ExecuteResultAsync(ActionContext context)
            {
                if (lastModified != default)
                    context.HttpContext.Response.Headers.Append("Last-Modified", new Microsoft.Extensions.Primitives.StringValues(lastModified.ToUniversalTime().ToString("R")));

                Task result = this.actionResult.ExecuteResultAsync(context);

                return result;
            }
        }
    }
}
