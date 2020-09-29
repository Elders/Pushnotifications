using System;
using System.Threading.Tasks;
using Elders.Cronus;
using Elders.Discovery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace PushNotifications.Api.Controllers.Discovery
{
    public class NormalizedDiscoveryController : ApiControllerBase
    {
        private readonly ApiResponse response;
        private readonly IDiscoveryReader discoveryReader;
        private readonly BoundedContext boundedContext;

        public NormalizedDiscoveryController(ApiResponse response, IDiscoveryReader discoveryReader, IOptionsMonitor<BoundedContext> optionsMonitor)
        {
            this.response = response;
            this.discoveryReader = discoveryReader;
            this.boundedContext = optionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Normalized Discovery endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpHead, HttpGet, Route("Discovery/normalized")]
        public async Task<IActionResult> DiscoveryAsync()
        {
            DiscoveryResponse model = await discoveryReader.GetAsync(boundedContext.Name);
            return response.Success(model).SetLastModifiedHeader(DateTimeOffset.FromFileTime(model.UpdatedAt));
        }
    }
}
