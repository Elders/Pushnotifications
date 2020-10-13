using Elders.Cronus;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PushNotifications.Service
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingHandler> logger;

        public LoggingHandler(ILogger<LoggingHandler> logger)
        {
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            StringBuilder requestTrace = new StringBuilder();
            if (logger.IsEnabled(LogLevel.Information))
            {
                await AddRequestLogAsync(request, requestTrace).ConfigureAwait(false);
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                await AddResponseLogAsync(response, requestTrace);
            }

            logger.Info(() => requestTrace.ToString());

            return response;
        }

        private async Task AddRequestLogAsync(HttpRequestMessage request, StringBuilder builder)
        {
            builder.AppendLine("Request:");
            builder.AppendLine(request.ToString());
            if (request.Content != null)
            {
                builder.AppendLine(await request.Content.ReadAsStringAsync().ConfigureAwait(false));// TODO: Read in chunks => https://elanderson.net/2019/12/log-requests-and-responses-in-asp-net-core-3/
            }
        }

        private async Task AddResponseLogAsync(HttpResponseMessage response, StringBuilder builder)
        {
            builder.AppendLine("Response:");
            builder.AppendLine(response.ToString());
            if (response.Content != null)
            {
                builder.AppendLine(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
        }
    }
}
